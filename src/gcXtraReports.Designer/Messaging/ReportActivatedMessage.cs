using System.Linq;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class ReportActivatedMessage
    {
        public gcXtraReport MyReport { get; set; }

        public ReportActivatedMessage(gcXtraReport myReport)
        {
            MyReport = myReport;
        }
    }
}