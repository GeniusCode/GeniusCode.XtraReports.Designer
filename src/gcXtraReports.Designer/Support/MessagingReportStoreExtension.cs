using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Designer.Messaging;
using gcExtensions;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class MessagingReportStoreExtension : ReportStorageExtension
    {
        private readonly IEventAggregator _aggregator;
        private readonly string _rootDirectory;

        public MessagingReportStoreExtension(IEventAggregator aggregator, string rootDirectory)
        {
            _aggregator = aggregator;
            _rootDirectory = rootDirectory;
        }

        protected override string RootDirectory
        {
            get { return _rootDirectory; }
        }

        public override void SetData(XtraReport report, Stream stream)
        {
            _aggregator.Publish(new ReportSavingMessage(report, null));
            base.SetData(report, stream);
        }
        public override void SetData(XtraReport report, string url)
        {
            _aggregator.Publish(new ReportSavingMessage(report, url));
            base.SetData(report, url);
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            _aggregator.Publish(new ReportSavingMessage(report, defaultUrl));
            return base.SetNewData(report, defaultUrl);
        }

    }


    /*  public class RelativePathReportStorage : ReportStorageExtension
    {
        readonly string _relativeBasePath;
        readonly string _executingAssemblyDirectory;
        readonly string _fullBasePath;

        public RelativePathReportStorage(string relativeReportBasePath)
        {
            _relativeBasePath = relativeReportBasePath;
            _executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var fullBasePathUgly = Path.Combine(_executingAssemblyDirectory, _relativeBasePath);
            _fullBasePath = new DirectoryInfo(fullBasePathUgly).FullName;
        }

        #region Open

/*        public override byte[] GetData(string url)
        {
            var convertedPath = ConvertToFullPath(url);
            return base.GetData(convertedPath);
        }#1#

/*        // Gets URL to open. Really an open file dialog.
        public override string GetNewUrl()
        {
            var filename = _absolutePathGetter();
            if (string.IsNullOrWhiteSpace(filename)) return string.Empty;
            // Convert full path to relative path
            var selectedRelativeFilePath = ConvertToRelativePath(filename);
            return selectedRelativeFilePath;
        }#1#

        #endregion


        #region Save

        // Save New
        public override string SetNewData(DevExpress.XtraReports.UI.XtraReport report, string defaultUrl)
        {
            var convertedURL = ConvertToFullPath(defaultUrl);
            return base.SetNewData(report, convertedURL);
        }

        // Save Existing
        public override void SetData(DevExpress.XtraReports.UI.XtraReport report, string url)
        {
            var convertedURL = ConvertToFullPath(url);
            base.SetData(report, convertedURL);
        }

        #endregion

        public override bool IsValidUrl(string url)
        {
            var convertedURL = ConvertToFullPath(url);
            return base.IsValidUrl(convertedURL);
        }

        #region Helpers

        private string ConvertToFullPath(string relativePath)
        {
            // Strip ~\
            var toRemove = @"~\{0}\".FormatString(_relativeBasePath);
            relativePath = relativePath.Replace(toRemove, "");

            return Path.Combine(_relativeBasePath, relativePath);
        }

        private string ConvertToRelativePath(string fullPath)
        {
            var toRemove = @"{0}\".FormatString(_fullBasePath);
            return fullPath.Replace(toRemove, "");
        }

        private bool isFullPathWithinBasePath(string testFullPath)
        {
            return testFullPath.StartsWith(_fullBasePath);
        }

        #endregion
    }*/
}
