using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Caliburn.Micro;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Design.Traversals;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Popups;
using GeniusCode.XtraReports.Designer.Repositories;
using GeniusCode.XtraReports.Designer.Support;
using GeniusCode.XtraReports.Runtime.Support;
using NLog;
using gcExtensions;

//using SelectDesignTimeDataSourceForm = XtraSubReport.Winforms.Popups.SelectDesignTimeDataSourceForm;

namespace GeniusCode.XtraReports.Designer
{
    public static class Program
    {
        public static ActionMessageHandler ActionMessageHandler;
        public static DebugMessageHandler DebugDebugMessageHandler;
        private const string DefaultRootFolderName = "gcXtraReports\\ReportDesigner";
        private const string DataSourceDirectoryName = "Datasources";
        private const string ReportsDirectoryName = "Reports";
        private const string ActionsDirectoryName = "Actions";
        private const string BootStrapperBatchFileName = "bootstrapper.bat";
        internal static string ProjectPath;
        internal static string ProjectReportPath;
        // NLog - Helpful to diagnose if a DLL cannot be found, etc.
        private static Logger _logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupNLog();
            
            ProjectBootStrapper projectBootstrapper;
            if (InitUsingBootstrappers(out projectBootstrapper) == false) return;

           
            CompositeRoot.Init(BuildContainer());
            var form = CompositeRoot.Instance.GetDesignForm();
            DebugDebugMessageHandler = CompositeRoot.Instance.GetDebugMessageHandler();
            ActionMessageHandler = CompositeRoot.Instance.GetActionMessageHandler();
 
            var extension = new CustomRootDirectoryStorageExtension(ProjectReportPath);
            ReportStorageExtension.RegisterExtensionGlobal(extension);

            DevExpress.Skins.SkinManager.EnableFormSkinsIfNotVista();
            //Here we specify the skin to use by its name   		

            Application.Run(form);
        }

        private static bool InitUsingBootstrappers(out ProjectBootStrapper projectBootstrapper)
        {
            projectBootstrapper = null;

            var bs = new AppBootStrapper(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DefaultRootFolderName));


            var mode = bs.DetectProjectMode();

            switch (mode)
            {
                case AppProjectsStructureMode.None:
                    new NoProjectsExistWarning(bs).ShowDialog();
                    return false;
                case AppProjectsStructureMode.MultipleUnchosen:
                    new ChooseProject(bs).ShowDialog();
                    if (bs.DetectProjectMode() == AppProjectsStructureMode.MultipleUnchosen)
                        return false;
                    break;
            }
            var debug = new TraceOutput(EventAggregatorSingleton.Instance);
            debug.Show();
            projectBootstrapper = bs.GetProjectBootstrapper(ReportsDirectoryName, DataSourceDirectoryName,
                                                                ActionsDirectoryName);
            
            projectBootstrapper.ExecuteProjectBootStrapperFile(BootStrapperBatchFileName);
            projectBootstrapper.CopyProjectFiles();
            projectBootstrapper.LoadProjectAssemblies();
            
            ProjectPath = projectBootstrapper.ProjectPath;
            ProjectReportPath = Path.Combine(ProjectPath, ReportsDirectoryName);
            return true;
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var actionTypes = (from a in AppDomain.CurrentDomain.GetAssemblies()
                               from t2 in a.GetTypes()
                               where typeof (IReportControlAction).IsAssignableFrom(t2) && ! t2.IsAbstract
                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Runtime.dll"
                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Design.dll"
                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Designer.dll"
                               select t2).ToList();

            var datasourceProviderTypes = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                           from t2 in a.GetTypes()
                                           where
                                               typeof (IReportDatasourceFactory).IsAssignableFrom(t2) && !t2.IsAbstract
                                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Runtime.dll"
                                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Design.dll"
                                               && t2.Assembly.ManifestModule.Name != "gcXtraReports.Designer.dll"
                                           select t2).ToList();
            

            actionTypes.ForEach(t=> builder.RegisterType(t).AsImplementedInterfaces());
            datasourceProviderTypes.ForEach(t => builder.RegisterType(t).AsImplementedInterfaces());

            builder.RegisterInstance(EventAggregatorSingleton.Instance).AsImplementedInterfaces();
            builder.RegisterType<MessagingDesignForm>().OnActivated(a=> DrawToolbarButtons(a.Instance));
            builder.RegisterType<DesignReportMetadataAssociationRepository>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DesignDataRepository>().AsImplementedInterfaces().SingleInstance();
            //TODO: Make this work again builder.RegisterType<SelectDesignTimeDataSourceForm>();
            builder.RegisterType<DesignDataContext2>().SingleInstance();
            builder.RegisterType<ActionMessageHandler>().SingleInstance();
            builder.RegisterType<DebugMessageHandler>().SingleInstance();
            builder.RegisterType<ReportControllerFactory>().AsImplementedInterfaces();
            builder.RegisterType<ObjectGraphPathTraverser>().AsImplementedInterfaces();
            builder.RegisterType<DataSourceSetter>().AsImplementedInterfaces();
            return builder.Build();
        }

        private static void DrawToolbarButtons(MessagingDesignForm form)
        {
            var dataContext = CompositeRoot.Instance.GetDesignDataContext();

            var item = new BarButtonItem(form.DesignBarManager, "See Messages");

            // Click Handler
            item.ItemClick += (s, e) => new ShowMessages(DebugDebugMessageHandler).ShowDialog();

            // Add Datasource Button
            form.DesignBarManager.Toolbar.AddItem(item);


            item = new BarButtonItem(form.DesignBarManager, "Select Datasource...");

            // Click Handler
            item.ItemClick += (s, e) =>
            {
                if (form.DesignMdiController.ActiveDesignPanel == null)
                {
                    MessageBox.Show("Please create/open a report.");
                    return;
                }

                var report = form.DesignMdiController.ActiveDesignPanel.Report;
                

                                                   PromptSelectDatasource(form, report, dataContext);
                                                   form.RedrawFieldListOnActiveDesignPanel();
            };

            // Add Datasource Button
            form.DesignBarManager.Toolbar.AddItem(item);


            // Hide Scripting & HTML Preview
            form.DesignMdiController.SetCommandVisibility(ReportCommand.ShowScriptsTab, CommandVisibility.None);
            form.DesignMdiController.SetCommandVisibility(ReportCommand.ShowHTMLViewTab, CommandVisibility.None);

        }


        


        private static void PromptSelectDatasource(XRDesignForm form, XtraReport report, IDesignDataContext dataContext)
        {
            Form dialog = null;


            // Create Select Datasource Dialog
            dialog = new SelectDesignTimeDataSourceForm(dataContext, report, EventAggregatorSingleton.Instance, new ObjectGraphPathTraverser());
            dialog.BringToFront();
            dialog.ShowDialog();
        }


        private static void SetupNLog()
        {
            //ConfigurationItemFactory.Default.Targets.RegisterDefinition("MessagePublishingTarget", typeof(MessagePublishingTarget));

            // Create a Logger
            //var target = new MessagePublishingTarget();
            //ogManager.Configuration.AddTarget("MessagePublishingTarget", target);
            //ogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            _logger = LogManager.GetCurrentClassLogger();

            // Add the event handler for handling non-UI thread exceptions 
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                _logger.FatalException("Report Designer encountered unhandled exception", exception);
            };
        }




    }
}
