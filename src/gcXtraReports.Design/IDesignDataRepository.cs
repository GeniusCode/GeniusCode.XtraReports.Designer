using System.Collections.Generic;
using System.Linq;

namespace GeniusCode.XtraReports.Design
{
    public interface IDesignDataRepository
    {
        IEnumerable<IReportDatasourceMetadata> GetAvailableMetadatas();
        object GetDataSourceByUniqueId(string uniqueId);
        IReportDatasourceMetadata GetDataSourceMetadataByUniqueId(string uniqueId);
    }

    
}