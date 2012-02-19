using System.Linq;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Design.Datasources;
using GeniusCode.XtraReports.Runtime;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class DataSourceSetter : IDataSourceSetter
    {
        private readonly IDesignDataRepository _designDataRepository;
        private readonly IDesignReportMetadataAssociationRepository _reportMetadataAssociationRepository;
        private readonly IDataSourceTraverser _dataSourceTraverser;

        public DataSourceSetter(IDesignDataRepository designDataRepository, 
                                      IDesignReportMetadataAssociationRepository reportMetadataAssociationRepository,
            IDataSourceTraverser dataSourceTraverser)
        {
            _designDataRepository = designDataRepository;
            _reportMetadataAssociationRepository = reportMetadataAssociationRepository;
            _dataSourceTraverser = dataSourceTraverser;
        }

        public void SetReportDatasource(gcXtraReport report, IReportDatasourceMetadata md)
        {
            SetReportDatasource(report,md,string.Empty);
        }

        public void SetReportDatasource(gcXtraReport report, IReportDatasourceMetadata md, string traversalPath)
        {
            //Fetch datasource from repository
            var datasourceObject = _designDataRepository.GetDataSourceByUniqueId(md.UniqueId);

            //Traverse path
            var traverseResult = _dataSourceTraverser.TraversePath(datasourceObject, traversalPath);
            //Set Datasource
            report.SetReportOnDataSourceAsCollection(traverseResult.TraversedDataSource);

            //Store association          
            var mdWithTraversal = new ReportDatasourceMetadataWithTraversal(md, traversalPath,
                                                                                  traverseResult.TraversedDataType);
            _reportMetadataAssociationRepository.AssociateWithReportAsCurrent(mdWithTraversal,report);           
        }



    }
}