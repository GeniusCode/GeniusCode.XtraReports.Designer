using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using FluentAssertions;
using GeniusCode.XtraReports;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Design.Traversals;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Repositories;
using GeniusCode.XtraReports.Designer.Support;
using GeniusCode.XtraReports.Runtime;
using GeniusCode.XtraReports.Runtime.Support;
using NUnit.Framework;
using XtraSubReports.TestResources.Infrastructure;
using XtraSubReports.TestResources.Models;
using XtraSubReports.TestResources.Reports;

namespace XtraSubReports.Winforms.Tests.Integration
{


 
    [TestFixture]
    public class Setter_tests
    {


        
        [Test]
        public void Should_do_everything()
        {
            //TODO: rewrite as spec or individual unit tests!

            // given infrastructure
            DataSourceSetter setter;
            IDesignDataRepository datarep;
            var dataDefRep = init(out setter, out datarep);
            var handler = new ActionMessageHandler( setter, new EventAggregator(), datarep,dataDefRep, new ReportControllerFactory());
            
            // given a report
            var report = new XtraReportWithSubReportInDetailReport();
            var report2 = report.CloneLayoutAsMyReportBase();

            

            // given the parent has a datasource
            IReportDatasourceMetadata metadata = datarep.GetAvailableMetadatas().Single(a => a.UniqueId == "DogTime");
            setter.SetReportDatasource(report2, metadata);
            
            // given a subreport in parent report
            var newSubReport = new gcXtraReport();
            var band = (DetailReportBand) report2.Bands[BandKind.DetailReport];
            var myContainer = (XRSubreport) band.Bands[BandKind.Detail].Controls[0];
            
            
            // when handling a message
            var message = new ReportActivatedBySubreportMessage(newSubReport, myContainer);
            handler.Handle(message);

            // then:
            newSubReport.DataSource.Should().NotBeNull();
            var dog = (Dog)((List<object>) newSubReport.DataSource).Single();
            var peoples = (List<Person2>) report2.DataSource;

            peoples[0].Dogs[0].Name.Should().Be(dog.Name);
        }

        private static DesignReportMetadataAssociationRepository init(out DataSourceSetter setter,
                                                                          out IDesignDataRepository datarep)
        {
            var providers = new List<IReportDatasourceFactory> {new DogTimeReportDatasourceProvider()};
            var dataDefRep = new DesignReportMetadataAssociationRepository();
            datarep = new DesignDataRepository(providers);
            setter = new DataSourceSetter(datarep, dataDefRep, new ObjectGraphPathTraverser());
            return dataDefRep;
        }


        [Test]
        public void Should_set_datasource_on_nontraversal()
        {
            DataSourceSetter setter;
            IDesignDataRepository datarep;
            DesignReportMetadataAssociationRepository dataDefRep = init(out setter, out datarep);

            var md = datarep.GetDataSourceMetadataByUniqueId("DogTime");
            var report = new gcXtraReport();
            setter.SetReportDatasource(report, md);
            report.DataSource.Should().NotBeNull();
            var persons = ((List<Person2>)report.DataSource);

            persons.Count().Should().Be(3);
        }

        [Test]
        public void Should_set_datasource_on_traversal()
        {
            DataSourceSetter setter;
            IDesignDataRepository datarep;
            var dataDefRep = init(out setter, out datarep);

            var md = datarep.GetDataSourceMetadataByUniqueId("DogTime");
            var report = new gcXtraReport();
            setter.SetReportDatasource(report, md,"Dogs");
            report.DataSource.Should().NotBeNull();
            var dogs = ((List<Dog>) report.DataSource);

            dogs.Count().Should().Be(2);
        }

    }


}