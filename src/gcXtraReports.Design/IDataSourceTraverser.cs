using System.Linq;
using GeniusCode.XtraReports.Design.Traversals;

namespace GeniusCode.XtraReports.Design
{
    public interface IDataSourceTraverser
    {
        TraversedDatasourceResult TraversePath(object datasource, string path);
    }
}