using System.Linq;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Design
{
    public interface IDataSourceSetter
    {
        void SetReportDatasource(gcXtraReport report, IReportDatasourceMetadata md);
        void SetReportDatasource(gcXtraReport report, IReportDatasourceMetadata md, string traversalPath);
    }
}