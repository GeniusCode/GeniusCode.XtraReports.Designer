using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Design
{
    public interface IDataSourceSetter
    {
        void SetReportDatasource(XtraReport report, IReportDatasourceMetadata md);
        void SetReportDatasource(XtraReport report, IReportDatasourceMetadata md, string traversalPath);
    }
}