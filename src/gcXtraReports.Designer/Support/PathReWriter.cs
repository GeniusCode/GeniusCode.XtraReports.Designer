using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DevExpress.XtraReports.UI;
using GeniusCode.Components.RelayVisitor;
using GeniusCode.XtraReports.Designer.Messaging;
using gcExtensions;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class PathReWriter
    {
        private readonly string _basePath;
        private readonly RelayVisitor<XtraReport> _visitor;

        public PathReWriter(string basePath)
        {
            _basePath = basePath;
            _visitor =  GetVisitor();
        }

        private RelayVisitor<XtraReport> GetVisitor()
        {
            var visitor = new RelayVisitor<XtraReport>(true);
          visitor.AddScoutForRootType(rt => rt.Controls.OfType<XRControl>());
          visitor.AddScoutForRootType(rt=> rt.Bands.OfType<Band>());
          visitor.AddScoutForType<Band,IEnumerable<XRControl>>(b=> b.Controls.OfType<XRControl>(),a=> true);
          
          visitor.AddActionForType<XRSubreport>(sr=> sr.ReportSourceUrl = ConvertToRelativePath(sr.ReportSourceUrl),a=> true);
            return visitor;
        }

        public void PerformOnReport(XtraReport report)
        {
            _visitor.Go(report);          
        }


        private string ConvertToRelativePath(string fullPath)
        {
            //TODO: test
            if (fullPath.StartsWith("~\\")) return fullPath;


            if(!fullPath.StartsWith(_basePath))
                throw new InvalidOperationException("Path for subreport in not constrained within base path!");

            var toRemove = @"{0}\".FormatString(_basePath);
            return string.Format("~\\{0}", fullPath.Replace(toRemove, ""));
        }
    }
}
