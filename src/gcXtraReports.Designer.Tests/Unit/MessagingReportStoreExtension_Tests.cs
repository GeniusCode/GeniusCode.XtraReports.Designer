using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using FluentAssertions;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Support;
using NUnit.Framework;

namespace GeniusCode.XtraReports.Designer.Tests.Unit
{

    [TestFixture]
    public class MessagingReportStoreExtension_Tests
    {

        private class DummyHandler : IHandle<ReportSavingMessage>
        {
            public DummyHandler(IEventAggregator aggregator)
            {
                aggregator.Subscribe(this);
            }

            public void Handle(ReportSavingMessage message)
            {
                Count++;
            }

            public int Count { get; set; }
        }

        [Test]
        public void ShouldSaveFileAndSendMessage()
        {
            var aggregator = new EventAggregator();
            var ext = new MessagingReportStoreExtension(aggregator, "");
            var handler = new DummyHandler(aggregator);
            var report = new XtraReport();

            var path = GetNewTempPath();

            var newFilePath = path + "\\" + "newReport.repx";
            ext.SetData(report,newFilePath);
            File.Exists(newFilePath).Should().BeTrue();

            handler.Count.Should().Be(1);
        }

        private string GetNewTempPath()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
