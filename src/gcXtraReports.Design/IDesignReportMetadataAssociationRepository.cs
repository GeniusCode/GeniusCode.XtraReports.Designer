using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Design
{
    public interface IDesignReportMetadataAssociationRepository
    {
        IEnumerable<IReportDatasourceMetadataWithTraversal> GetAssociationsForReport(XtraReport report);
        IReportDatasourceMetadataWithTraversal GetCurrentAssociationForReport(XtraReport report);
        void AssociateWithReport(IReportDatasourceMetadataWithTraversal definition, XtraReport report);
        void AssociateWithReportAsCurrent(IReportDatasourceMetadataWithTraversal definition, XtraReport report);
    }
}