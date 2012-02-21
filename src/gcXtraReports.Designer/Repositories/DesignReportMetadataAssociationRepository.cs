using System.Collections.Generic;
using System.Linq;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Runtime.Support;
using gcExtensions;
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
            return _allItemsDictionary.GetOrCreateValue(report.GetHashCode(),
                                                        () => new HashSet<IReportDatasourceMetadataWithTraversal>());
        }

        public IReportDatasourceMetadataWithTraversal GetCurrentAssociationForReport(gcXtraReport report)
        {
            IReportDatasourceMetadataWithTraversal output;
            _currentlySelectedDictionary.TryGetValue(report.GetHashCode(), out output);
            return output;
        }

        public void AssociateWithReport(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report)
        {
            var hashset = _allItemsDictionary.GetOrCreateValue(report.GetHashCode(),
                                                               () => new HashSet<IReportDatasourceMetadataWithTraversal>());
            if (!hashset.Contains(definition))
                hashset.Add(definition);
        }

        public void AssociateWithReportAsCurrent(IReportDatasourceMetadataWithTraversal definition, gcXtraReport report)
        {
            _currentlySelectedDictionary.AddIfUniqueOrReplace(definition, report.GetHashCode());
        }
    }
}