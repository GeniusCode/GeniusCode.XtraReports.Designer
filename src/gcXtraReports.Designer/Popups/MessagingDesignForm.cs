using System;
using System.Linq;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Prototypes;
using GeniusCode.XtraReports.Runtime.Support;
using gcExtensions;

namespace GeniusCode.XtraReports.Designer.Popups
{
    public class MessagingDesignForm : XRDesignForm
    {
        #region fields

        private readonly IEventAggregator _aggregator;
        private readonly IDesignReportMetadataAssociationRepository _designReportMetadataAssociationRepository;

        #endregion

        #region Constructors

        public MessagingDesignForm(IEventAggregator aggregator, IDesignReportMetadataAssociationRepository designReportMetadataAssociationRepository)
        {
            if (aggregator == null) throw new ArgumentNullException("aggregator");
            _aggregator = aggregator;
            _designReportMetadataAssociationRepository = designReportMetadataAssociationRepository;

            InitForm();
        }


        #endregion

        private void InitForm()
        {
            AssignHandlers();
            SendMessagesOnReportActivations();
        }

        private void AssignHandlers()
        {
            DesignMdiController.DesignPanelLoaded -= DesignMdiController_DesignPanelLoaded;
            DesignMdiController.DesignPanelLoaded += DesignMdiController_DesignPanelLoaded;
        }

        #region Event Handlers

        void DesignMdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {
            RegisterPanelForMessageOnPreview(sender);
        }       

        #endregion

        #region Assets

        private void RegisterPanelForMessageOnPreview(object sender)
        {
            var message = new DesignPanelPrintPreviewMessage((XRDesignPanel)sender);
            var handler = new RelayCommandHandler((rc, oa) =>
            {
                _aggregator.Publish(message);
                return true;
            }
                                                  ,
                                                  rc => rc == ReportCommand.ShowPreviewTab);

            DesignMdiController.AddCommandHandler(handler);
        }

        private void SendMessagesOnReportActivations()
        {
            // Selected Subreport on Design Panel
            SubreportBase selectedSubreport = null;

            // Design-Time Event Handler
            OnDesignPanelFirstTimeActivated(designPanel =>
            {
                var report = designPanel.Report;

                // Populate Design-Time Datasource
                report.TryAs<XtraReport>(myReport =>
                {
                    if (selectedSubreport == null)
                        // Stand-alone Report
                        _aggregator.Publish(new ReportActivatedMessage(myReport));
                    //TODO: Store design time datasource in xml file (later on)
                    //ChangeDesignTimeDatasourceToDefault(myReport);
                    else
                        // Subreport was Double-clicked, new DesignPanel has been activated for it
                        // Pass Design-Time DataSource from Parent to Subreport
                        _aggregator.Publish(new ReportActivatedBySubreportMessage(myReport, selectedSubreport));
                        this.RedrawFieldListOnDesignPanel(designPanel);
                    //PassDesignTimeDataSourceToSubreport(selectedSubreport, myReport);
                });

                // Capture selected Subreport
                // So we can enable double-clicking Subreport
                designPanel.SelectionChanged += (sender, e) =>
                {
                    var selected = designPanel.GetSelectedComponents();
                    selectedSubreport = selected.OfType<SubreportBase>().SingleOrDefault();
                };

            });
        }

        private void OnDesignPanelFirstTimeActivated(Action<XRDesignPanel> handler)
        {
            DesignMdiController.DesignPanelLoaded += (sender1, designLoadedArgs) =>
            {
                var designPanel = (XRDesignPanel)sender1;

                EventHandler activated = null;
                activated = (sender2, emptyArgs) =>
                {
                    // remove handler after the first time it is activated
                    designLoadedArgs.DesignerHost.Activated -= activated;
                    handler.Invoke(designPanel);
                };

                designLoadedArgs.DesignerHost.Activated += activated;
            };
        }

/*        /// <summary>
        /// Concatenates all nested DataMember paths to create the Full DataMember Path
        /// </summary>
        /// <param name="band"></param>
        /// <returns></returns>
        private string GetFullDataMemberPath(Band band)
        {
            string path = String.Empty;

            if (band == null)
                return path;

            band.TryAs<XtraReportBase>(report =>
            {
                var context = report.GetReportDataContext();
                path = context.GetDataMemberDisplayName(report.DataSource, report.DataMember);

                bool isRootReport = report == report.Report;

                // Keep calling parent reports, until there is no DataMember
                if (!isRootReport && path != String.Empty)
                {
                    // Go deeper
                    string parentPath = GetFullDataMemberPath(report.Report);
                    path = "{0}.{1}".FormatString(parentPath, path);

                    // Prevent a period at the beginning
                    if (path.StartsWith("."))
                        path = path.Remove(0, 1);
                }
                else
                {
                    // If we are at the top, add starting Relation Path from datasource
                    report.TryAs<gcXtraReport>(myReport =>
                                                   {
                                                       var selectedDatasourceDefinition =
                                                           _designReportMetadataAssociationRepository.GetCurrentAssociationForReport(myReport);

                        if (selectedDatasourceDefinition != null)
                        {
                            // Append parent report
                            var startingReportPath = selectedDatasourceDefinition.TraversalPath;
                            path = startingReportPath;
                        }
                    });
                }
            });

            if (band is XtraReportBase == false)
            {
                var report = band.Report;

                // Recurse into Report, which has the DataMember for the Collection
                // Prevent infinite loop, when a report is it's own parent
                if (band != report)
                    path = GetFullDataMemberPath(report);
            }

            band.TryAs<DetailBand>(detail =>
            {
                // Detail band is a single item
                // By Default, Use first element for design-time
                path += "[0]";
            });

            return path;
        }*/

        #endregion

    }
}