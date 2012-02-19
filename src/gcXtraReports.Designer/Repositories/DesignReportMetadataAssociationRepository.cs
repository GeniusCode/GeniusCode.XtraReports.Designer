using System.Collections.Generic;
using System.Linq;
using GeniusCode.Framework.Extensions;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Repositories
{
    public class DesignReportMetadataAssociationRepository : IDesignReportMetadataAssociationRepository
    {
        private readonly Dictionary<int, IReportDatasourceMetadataWithTraversal> _currentlySelectedDictionary;
        private readonly Dictionary<int, HashSet<IReportDatasourceMetadataWithTraversal>> _allItemsDictionary;

        public DesignReportMetadataAssociationRepository()
        {
            _currentlySelectedDictionary = new Dictionary<int, IReportDatasourceMetadataWithTraversal>();
            _allItemsDictionary = new Dictionary<int, HashSet<IReportDatasourceMetadataWithTraversal>>();
        }

        public IEnumerable<IReportDatasourceMetadataWithTraversal> GetAssociationsForReport(gcXtraReport report)
        {
            return _allItemsDictionary.CreateOrGetValue(report.GetHashCode(),
                                                        () => new HashSet<IReportDatasourceMetadataWithTraversal>());
        }

        public IReportDatasourceMetadataWithTraversal GetCurrentAssociationForReport(gcXtraReport report)
        {
            return _currentlySelectedDictionary.GetIfExists(report.GetHashCode(), null);
        }

        public void AssociateWithReport(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report)
        {
            var hashset = _allItemsDictionary.CreateOrGetValue(report.GetHashCode(),
                                                               () => new HashSet<IReportDatasourceMetadataWithTraversal>());

            hashset.AddIfUnique(definition);
        }

        public void AssociateWithReportAsCurrent(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report)
        {
            _currentlySelectedDictionary.AddIfUniqueOrReplace(definition, ds => report.GetHashCode());
        }
    }
}