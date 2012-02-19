using System.Linq;
using GeniusCode.XtraReports.Design.Datasources;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class DataSourceSelectedForReportMessage
    {
        public IReportDatasourceMetadataWithTraversal ReportDatasourceMetadataWithTraversal { get; set; }
        public gcXtraReport Report { get; set; }

        public DataSourceSelectedForReportMessage(IReportDatasourceMetadataWithTraversal reportDatasourceMetadataWithTraversal, gcXtraReport report)
        {
            ReportDatasourceMetadataWithTraversal = reportDatasourceMetadataWithTraversal;
            Report = report;
        }
    }
}