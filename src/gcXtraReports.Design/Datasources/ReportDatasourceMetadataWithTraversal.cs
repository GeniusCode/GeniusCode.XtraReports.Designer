using System;
using System.Linq;


namespace GeniusCode.XtraReports.Design.Datasources
{
    public class ReportDatasourceMetadataWithTraversal : IReportDatasourceMetadataWithTraversal
    {
        public ReportDatasourceMetadataWithTraversal(IReportDatasourceMetadata md, string traversalPath, Type traversedDataSourceType)
        {
            UniqueId = md.UniqueId;
            Name = md.Name;
            Description = md.Description;
            DataSourceType = md.DataSourceType;
            TraversalPath = traversalPath;
            TraversedDataSourceType = traversedDataSourceType;
        }

        public string UniqueId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Type DataSourceType { get; private set; }

        public override string ToString()
        {
            return "{0} ('{1}')".FormatString(Name, TraversalPath);
        }

        public string TraversalPath { get; set; }

        public Type TraversedDataSourceType { get; private set; }
    }
}
