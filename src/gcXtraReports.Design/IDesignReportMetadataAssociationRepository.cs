using System.Collections.Generic;
using System.Linq;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Design
{
    public interface IDesignReportMetadataAssociationRepository
    {
        IEnumerable<IReportDatasourceMetadataWithTraversal> GetAssociationsForReport(gcXtraReport report);
        IReportDatasourceMetadataWithTraversal GetCurrentAssociationForReport(gcXtraReport report);
        void AssociateWithReport(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report);
        void AssociateWithReportAsCurrent(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report);
    }
}