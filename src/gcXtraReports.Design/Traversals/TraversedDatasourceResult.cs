using System;
using System.Linq;

namespace GeniusCode.XtraReports.Design.Traversals
{
    public class TraversedDatasourceResult
    {
        public object RootDataSource { get; private set; }
        public object TraversedDataSource { get; private set; }

        public Type TraversedDataType {get { return Succeeded ? TraversedDataSource.GetType() : typeof (object); }}

        public bool Succeeded
        {
            get { return RootDataSource != null && TraversedDataSource != null; }
        }

        public TraversedDatasourceResult(object rootDataSource, object traversedDataSource)
        {
            RootDataSource = rootDataSource;
            TraversedDataSource = traversedDataSource;
        }
    }
}