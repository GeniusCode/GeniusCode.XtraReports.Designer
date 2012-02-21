using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Design.Datasources;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class DataSourceSelectedForReportMessage
    {
        public IReportDatasourceMetadataWithTraversal ReportDatasourceMetadataWithTraversal { get; set; }
        public XtraReport Report { get; set; }

        public DataSourceSelectedForReportMessage(IReportDatasourceMetadataWithTraversal reportDatasourceMetadataWithTraversal, XtraReport report)
        {
            ReportDatasourceMetadataWithTraversal = reportDatasourceMetadataWithTraversal;
            Report = report;
        }
    }
}