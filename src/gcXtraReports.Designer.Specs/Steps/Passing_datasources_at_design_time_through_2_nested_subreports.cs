using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using FluentAssertions;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Support;
using TechTalk.SpecFlow;
using XtraSubReports.TestResources.Infrastructure;
using XtraSubReports.TestResources.Models;
using XtraSubReports.TestResources.Reports;

namespace GeniusCode.XtraReports.Designer.Specs.Steps
{

    [Binding]
    [Scope(Feature = "Passing datasources at design time", Scenario = "Passing Datasource Using 2-Nested Subreports")]
    public class Passing_datasources_at_design_time_through_2_nested_subreports
    {
        IDataSourceSetter _setter;
        IDesignDataContext _dataContext;
        private IReportDatasourceMetadata _datasourceMetadata;
        private XtraReport _reportA;

        private XRSubreport _containerwithReportBinside;
        private XtraReport _reportB;

        private XRSubreport _containerWithReportCInside;
        private XtraReport _reportC;

        private ActionMessageHandler _messageHandler;

        private IEventAggregator _eventAggregator;

        [Given(@"The design runtime is ready")]
        public void GivenTheDesignRuntimeIsReady()
        {
            _eventAggregator = new EventAggregator();
            _dataContext = Factory.CreateForDogTime(out _setter);
            _messageHandler = new ActionMessageHandler(_setter, _eventAggregator, _dataContext.DesignDataDefinitionRepository, new ReportControllerFactory());
        }

        [Given(@"a datasource exists called DogTime")]
        public void GivenADatasourceExistsCalledDogTime()
        {
            _datasourceMetadata = _dataContext.DesignDataRepository.GetDataSourceMetadataByUniqueId("DogTime");
        }

        [Given(@"ReportA exists with a subreport called ReportB in a detail report")]
        public void GivenReportAExistsWithSubreportReportB()
        {
            _reportA = new XtraReportWithSubReportInDetailReport();
            var band = (DetailReportBand)_reportA.Bands[BandKind.DetailReport];
            _containerwithReportBinside = (XRSubreport)band.Bands[BandKind.Detail].Controls[0];
        }

        [Given(@"ReportB exists with a subreport called ReportC in a detail report")]
        public void GivenReportBExistsWithSubreportReportC()
        {
            _reportB = new XtraReportWithSubReportInDetailReport();
            var band = (DetailReportBand)_reportB.Bands[BandKind.DetailReport];
            band.DataMember = "DogToys";
            _containerWithReportCInside = (XRSubreport)band.Bands[BandKind.Detail].Controls[0];

            _reportC = new XtraReport();
        }

        [Given(@"a new instance of ReportA exists")]
        public void GivenANewInstanceOfReportAExists()
        {
            // nothing to do
        }

        [Given(@"ReportA loads the DogTime datasource")]
        public void GivenReportALoadsTheDogTimeDatasource()
        {
            _setter.SetReportDatasource(_reportA, _datasourceMetadata);
        }

        [Given(@"the user has activated subreport ReportB inside ReportA")]
        public void GivenTheUserHasActivatedSubreportReportBInsideReportA()
        {
            _eventAggregator.Publish(new ReportActivatedBySubreportMessage(_reportB, _containerwithReportBinside));
        }

        [When(@"the user activates subreport ReportC inside ReportB")]
        public void WhenTheUserActivatesSubreportReportCInsideReportB()
        {
            _eventAggregator.Publish(new ReportActivatedBySubreportMessage(_reportC, _containerWithReportCInside));
        }

        [Then(@"ReportC's datasource should be the first Toy of the first Dog of the first Person in DogTime")]
        public void ThenReportCSDatasourceShouldBeTheFirstToyOfTheFirstDogOfTheFirstPersonInDogTime()
        {
            var peoples = (List<Person2>)_reportA.DataSource;
            var dog = (Dog)((List<object>)_reportB.DataSource).Single();
            var dogToy = (DogToy)((List<object>)_reportC.DataSource).Single();

            dog.Name.Should().Be(peoples[0].Dogs[0].Name);
            dogToy.Name.Should().Be(dog.DogToys[0].Name);
        }


        //[Given(@"PersonReport exists with a subreport called DogReport in a detail report")]
        //public void GivenPersonReportExistsWithASubreportCalledDogReportInADetailReport()
        //{
        //    _parentReport = new XtraReportWithSubReportInDetailReport().ConvertReportToMyReportBase(_eventAggregator);

        //    var band = (DetailReportBand)_parentReport.Bands[BandKind.DetailReport];
        //    _container = (XRSubreport)band.Bands[BandKind.Detail].Controls[0];
        //}


        //[Given(@"PersonReport exists with a subreport called PersonSubReport in a detail band")]
        //public void GivenPersonReportExistsWithASubreportCalledPersonSubReportInADetailBand()
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Given(@"PersonReport loads the DogTime datasource")]
        //public void PersonReportLoadsTheDogTimeDatasource()
        //{
        //    _setter.SetReportDatasource(_parentReport, _datasourceMetadata);
        //}

        //[Given(@"a new report instance exists")]
        //public void GivenANewReportInstanceExists()
        //{
        //    _newReport = new XtraReport();
        //}

        //[When(@"A ReportActivatedBySubreportMessage occurs which contains the new report instance")]
        //public void WhenAReportActivatedBySubreportMessageOccursWhichContainsTheNewReportInstance()
        //{
        //    _eventAggregator.Publish(new ReportActivatedBySubreportMessage(_newReport, _container));
        //}


        //[Then(@"the new report instance's datasource should be the first dog of the first person from PersonReport's datasource")]
        //public void ThenTheNewReportInstanceSDatasourceShouldBeTheFirstDogOfTheFirstPersonFromPersonReportSDatasource()
        //{
        //    var dog = (Dog)((List<object>)_newReport.DataSource).Single();
        //    var peoples = (List<Person2>)_parentReport.DataSource;

        //    peoples[0].Dogs[0].Name.Should().Be(dog.Name);
        //}

        //[Then(@"the new report instance's datasource should be the first person from the PersonReport's datasource")]
        //public void ThenTheNewReportInstanceSDatasourceShouldBeTheFirstPersonFromThePersonReportSDatasource()
        //{
        //    ScenarioContext.Current.Pending();
        //}



    }
}
