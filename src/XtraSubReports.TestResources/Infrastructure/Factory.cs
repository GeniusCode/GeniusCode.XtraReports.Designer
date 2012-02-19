using System.Collections.Generic;
using GeniusCode.XtraReports;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Design.Traversals;
using GeniusCode.XtraReports.Designer;
using GeniusCode.XtraReports.Designer.Repositories;
using GeniusCode.XtraReports.Designer.Support;

namespace XtraSubReports.TestResources.Infrastructure
{
    public static class Factory
    {
        public static IDesignDataContext CreateForDogTime(out IDataSourceSetter setter)
        {
            var providers = new List<IReportDatasourceFactory> { (IReportDatasourceFactory) new DogTimeReportDatasourceProvider() };
            var dataDefRep = new DesignReportMetadataAssociationRepository();
            var datarep = new DesignDataRepository(providers);
            setter = new DataSourceSetter(datarep, dataDefRep, new ObjectGraphPathTraverser());
            return new DesignDataContext2(dataDefRep,datarep);
        }
    }
}