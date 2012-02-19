using System.Collections.Generic;
using System.Linq;
using GeniusCode.XtraReports;
using XtraSubReports.TestResources.Models;

namespace XtraSubReports.TestResources.Infrastructure
{
    public class DogTimeReportDatasourceProvider : IReportDatasourceFactory
    {
        public IEnumerable<IReportDatasourceMetadata> GetReportDatasources()
        {
            return new[] { new TestReportDatasourceMetadata("DogTime") };
        }

        public object GetReportDatasource(string uniqueId)
        {
            return Person2.SampleData();
        }
    }
}
