using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using NLog;

//using SelectDesignTimeDataSourceForm = XtraSubReport.Winforms.Popups.SelectDesignTimeDataSourceForm;

namespace GeniusCode.XtraReports.Designer
{
    public static class Program
    {
        public static IEventAggregator DefaultEventAggregator = new EventAggregator();
        public static ActionMessageHandler ActionMessageHandler;
        public static DebugMessageHandler DebugDebugMessageHandler;
        private const string RegistryPath = "SOFTWARE\\gcXtraReports.Designer";
        private const string DefaultRootFolderName = "gcXtraReports\\ReportDesigner";
        private const string DataSourceDirectoryName = "Datasources";
        private const string ReportsDirectoryName = "Reports";
        private const string ActionsDirectoryName = "Actions";
        private const string BootStrapperBatchFileName = "bootstrapper.bat";
        internal static string ProjectPath;
        internal static string ProjectReportPath;
        // NLog - Helpful to diagnose if a DLL cannot be found, etc.
        private static Logger _logger;
        public static string LogPath;

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

            // Register Reports Base Folder
            var extension = new CustomRootDirectoryStorageExtension(ProjectReportPath);
            ReportStorageExtension.RegisterExtensionGlobal(extension);

            DevExpress.Skins.SkinManager.EnableFormSkinsIfNotVista();
            //Here we specify the skin to use by its name   		

            Application.Run(form);
        }

        private static bool InitUsingBootstrappers(out ProjectBootStrapper projectBootstrapper)
        {
            projectBootstrapper = null;

            IRootPathAcquirer acquirer = new RegistryHelper(RegistryPath);
            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                                           DefaultRootFolderName);

            var bs = new AppBootStrapper(acquirer, defaultPath);

            var mode = bs.DetectProjectMode();

            switch (mode)
            {
                case AppProjectsStructureMode.None:
                    new NoProjectsExistWarning(bs).ShowDialog();
                    return false;
                case AppProjectsStructureMode.MultipleUnchosen:
                    var form = new ChooseProject(bs.GetProjects());
                    form.ShowDialog();

                    if (!String.IsNullOrWhiteSpace(form.SelectedPath))
                        bs.SetProjectName(form.SelectedPath);

                    if (bs.DetectProjectMode() == AppProjectsStructureMode.MultipleUnchosen)
                        return false;
                    break;
                case AppProjectsStructureMode.Single:
                    bs.SetProjectNameToSingle();
                    break;
            }
            var debug = new TraceOutput(DefaultEventAggregator);
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


        private static IEnumerable<Assembly> GetAssebliesToLookup()
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   where a.ManifestModule.Name != "gcXtraReports.Runtime.dll"
                         && a.ManifestModule.Name != "gcXtraReports.Design.dll"
                         && a.ManifestModule.Name != "gcXtraReports.Designer.dll"
                         && a.ManifestModule.Name != "gcXtraReports.Core.dll"
                   select a;
        }                
        private static List<Type> GetReportControlActionTypes()
        {
            return (from a in GetAssebliesToLookup()
                    from t2 in a.GetTypes()
                    where typeof(IReportControlAction).IsAssignableFrom(t2) && !t2.IsAbstract
                    select t2).ToList();
        }


        private static List<Type> GetDataSourceTypes()
        {
            return (from a in GetAssebliesToLookup()
                    from t2 in a.GetTypes()
                    where typeof (IReportDatasourceFactory).IsAssignableFrom(t2) && !t2.IsAbstract
                    select t2).ToList();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            List<Type> actionTypes;
            List<Type> datasourceProviderTypes;
            try
            {
                 actionTypes = GetReportControlActionTypes();
                 datasourceProviderTypes = GetDataSourceTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                _logger.Log(LogLevel.Fatal,"An error happened while loading exceptions");

                var exceptions = e.LoaderExceptions.ToList();

                for (int i = 0; i < exceptions.Count; i++)
                {
                    var exception = e.LoaderExceptions[i];

                    _logger.LogException(LogLevel.Fatal,
                                         String.Format("Loader Exception {0} of {1}", i + 1, exceptions.Count), exception);
                }
                
                throw;
            }
            


            actionTypes.ForEach(t => builder.RegisterType(t).AsImplementedInterfaces());
            datasourceProviderTypes.ForEach(t => builder.RegisterType(t).AsImplementedInterfaces());

            builder.RegisterInstance(DefaultEventAggregator).AsImplementedInterfaces();
            builder.RegisterType<MessagingDesignForm>().OnActivated(a => DrawToolbarButtons(a.Instance));
            builder.RegisterType<DesignReportMetadataAssociationRepository>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DesignDataRepository>().AsImplementedInterfaces().SingleInstance();
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
                form.RedrawFieldListOnDesignPanel(null);
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
            dialog = new SelectDesignTimeDataSourceForm(dataContext, report, DefaultEventAggregator, new ObjectGraphPathTraverser());
            dialog.BringToFront();
            dialog.ShowDialog();
        }


        private static void SetupNLog()
        {
            //write config file to the root!
            //gcXtraReports.Designer

            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("GeniusCode.XtraReports.Designer.NLog.config"))
            {
                if (File.Exists("NLog.config"))
                    File.Delete("NLog.config");

                using (var fs = File.Create("NLog.config"))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }


            //ConfigurationItemFactory.Default.Targets.RegisterDefinition("MessagePublishingTarget", typeof(MessagePublishingTarget));

            // Create a Logger
            //var target = new MessagePublishingTarget();
            //ogManager.Configuration.AddTarget("MessagePublishingTarget", target);
            //ogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            _logger = LogManager.GetCurrentClassLogger();

            var logPath = Path.Combine(Path.GetTempPath(), "gcXtraReport.Designer");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string fileName = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);


            GlobalDiagnosticsContext.Set("logdir", logPath);
            GlobalDiagnosticsContext.Set("logfilename", fileName);

            LogPath = logPath + "\\" + fileName;

            // Add the event handler for handling non-UI thread exceptions 
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                _logger.FatalException("Report Designer encountered unhandled exception", exception);
                MessageBox.Show("An exception has occured. Please check the log file at:" + LogPath);

            };
        }




    }
}
