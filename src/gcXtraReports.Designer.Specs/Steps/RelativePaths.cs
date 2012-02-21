using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using FluentAssertions;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Support;
using TechTalk.SpecFlow;

namespace GeniusCode.XtraReports.Designer.Specs.Steps
{

    
    
    [Binding]
    [Scope(Feature = "Making paths to external subreports relative")]
    public class RelativePaths
    {
        public class Handler : IHandle<ReportSavingMessage>
        {
            private readonly PathReWriter _reWriter;

            public Handler(string rootPath)
            {
                _reWriter = new PathReWriter(rootPath);
            }

            public void Handle(ReportSavingMessage message)
            {
                _reWriter.PerformOnReport(message.Report);
            }
        }

        private string CreateTempDir()
        {
            var rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(rootPath);
            return rootPath;
        }

        private string _rootPath;
        private XRSubreport _subreportContainer;
        private MessagingReportStoreExtension _messagingReportStoreExtension;
        private XtraReport _reportA;
        private XtraReport _reportB;
        private Handler _handler;
        private string _reportBAbsolutePath;
        private string _reportAAbsolutePath;
        private IEventAggregator _aggregator;
        private string _expectedRelativePath;

        private Exception _exception;
        private bool _exceptionExpected;

        [Given(@"Infrastructure is initialized")]
        public void GivenInfrastructureIsInitialized()
        {
            _rootPath = CreateTempDir();
           _aggregator = new EventAggregator();
           _handler = new Handler(_rootPath);
            _aggregator.Subscribe(_handler);
            _messagingReportStoreExtension = new MessagingReportStoreExtension(_aggregator, _rootPath);
        }

        [Given(@"reportA file exists in a folder")]
        public void GivenReportAFileExistsInAFolder()
        {
            _reportA = new XtraReport();
            _reportA.Bands.Add(new DetailBand());
            _subreportContainer = new XRSubreport();
            _reportA.Bands[0].Controls.Add(_subreportContainer);

            _reportAAbsolutePath = _rootPath + "\\" + "ReportA.repx";
            _reportA.SaveLayout(_reportAAbsolutePath);

            File.Exists(_reportAAbsolutePath).Should().BeTrue("Step completed successfuly");
        }

        [Given(@"reportB file exists in the same folder as reportA")]
        public void GivenReportBFileExistsInTheSameFolderAsReportA()
        {
            _reportB = new XtraReport();       
             _reportBAbsolutePath = _rootPath + "\\" + "ReportB.repx";
             _reportB.SaveLayout(_reportBAbsolutePath);
             File.Exists(_reportBAbsolutePath).Should().BeTrue("Step completed successfuly");

            _expectedRelativePath = "ReportB.repx";
        }

        [Given(@"reportB file exists in a subdirectory of reportA's path")]
        public void GivenReportBFileExistsInASubdirectoryOfReportASPath()
        {
            _reportB = new XtraReport();

            _reportBAbsolutePath = _rootPath +  "\\SubFolder\\" + "ReportB.repx";
            Directory.CreateDirectory(Path.GetDirectoryName(_reportBAbsolutePath));
            _reportB.SaveLayout(_reportBAbsolutePath);
            _expectedRelativePath = "SubFolder\\ReportB.repx";
            File.Exists(_reportBAbsolutePath).Should().BeTrue("Step completed successfuly");
        }

        [Given(@"reportB file exists outside of reportA's directory structure")]
        public void GivenReportBFileExistsOutsideOfReportASDirectoryStructure()
        {
            _reportB = new XtraReport();

            var path = CreateTempDir();

            _reportBAbsolutePath = path + "\\SubFolder\\" + "ReportB.repx";
            Directory.CreateDirectory(Path.GetDirectoryName(_reportBAbsolutePath));
            _reportB.SaveLayout(_reportBAbsolutePath);
            _exceptionExpected = true;
            File.Exists(_reportBAbsolutePath).Should().BeTrue("Step completed successfuly");
        }

        [Given(@"the container on ReportA contains the full path to ReportB")]
        public void GivenTheContainerOnReportAContainsTheFullPathToReportB()
        {
            _subreportContainer.ReportSourceUrl = _reportBAbsolutePath;
        }

        [When(@"the report is saved")]
        public void WhenTheReportIsSaved()
        {
            try
            {
                _messagingReportStoreExtension.SetData(_reportA, _reportAAbsolutePath);
            }
            catch (Exception ex)
            {
                _exception = ex;
                if (!_exceptionExpected)
                    throw;
            }
            
        }

        [Then(@"the url on the container should be a relative path, and not absolute")]
        public void ThenTheUrlOnTheContainerShouldBeARelativePathAndNotAbsolute()
        {
            _subreportContainer.ReportSourceUrl.Should().Be(_expectedRelativePath);
        }

        [Then(@"an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            _exception.Should().NotBeNull();
        }

        [Then(@"container's url should not be relative")]
        public void ThenContainerSUrlShouldNotBeRelative()
        {
            _subreportContainer.ReportSourceUrl.Should().Be(_reportBAbsolutePath);
        }

    }
}
