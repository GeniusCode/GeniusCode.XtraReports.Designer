
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using FluentAssertions;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Support;
using GeniusCode.XtraReports.Runtime;
using TechTalk.SpecFlow;
using XtraSubReports.TestResources.Infrastructure;
using XtraSubReports.TestResources.Reports;

namespace GeniusCode.XtraReports.Designer.Specs.Steps
{

    [Binding]
    [Scope(Feature = "Passing datasources at design time", Scenario = "Activating a Subreport without a Datasource")]
    public class Activating_a_Subreport_without_a_Datasource
    {
        private ActionMessageHandler _messageHandler;
        private IEventAggregator _eventAggregator;
        private IDesignDataContext _dataContext;
        IDataSourceSetter _setter;

        private XtraReport _reportA;
        private XRSubreport _containerThatContainsReportB;
        private XtraReport _reportB;

        [Given(@"The design runtime is ready")]
        public void GivenTheDesignRuntimeIsReady()
        {
            _eventAggregator = new EventAggregator();
            _dataContext = Factory.CreateForDogTime(out _setter);
            _messageHandler = new ActionMessageHandler(_setter, _eventAggregator, _dataContext.DesignDataDefinitionRepository, new ReportControllerFactory());
        }

        [Given(@"ReportA exists with a subreport called ReportB in a detail report")]
        public void GivenReportAExistsWithASubreportCalledReportBInADetailReport()
        {
            _reportA = new XtraReportWithSubReportInDetailReport().ConvertReportToMyReportBase(_eventAggregator);

            var band = (DetailReportBand)_reportA.Bands[BandKind.DetailReport];
            _containerThatContainsReportB = (XRSubreport)band.Bands[BandKind.Detail].Controls[0];

            _reportB = new XtraReport();
        }

        [When(@"the user activates subreport ReportB inside ReportA without a datasource")]
        public void WhenTheUserActivatesSubreportReportBInsideReportAWithoutADatasource()
        {
            _eventAggregator.Publish(new ReportActivatedBySubreportMessage(_reportB, _containerThatContainsReportB));
        }

        [Then(@"ReportB should open without a datasource")]
        public void ThenReportBShouldOpenWithoutADatasource()
        {
            _reportB.DataSource.Should().BeNull();
        }

    }
}
