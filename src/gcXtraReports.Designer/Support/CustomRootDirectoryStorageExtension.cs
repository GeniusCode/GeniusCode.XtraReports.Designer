using System.IO;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class CustomRootDirectoryStorageExtension : ReportStorageExtension
    {
        private readonly string _rootDirectory;

        public CustomRootDirectoryStorageExtension(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        protected override string RootDirectory
        {
            get { return _rootDirectory; }
        }

        public override void SetData(XtraReport report, string url)
        {
            // Change Report Display Name when Saving.  So users know what file each report tab represents
            report.DisplayName = Path.GetFileNameWithoutExtension(url);

            base.SetData(report, url);
        }

    }
}
