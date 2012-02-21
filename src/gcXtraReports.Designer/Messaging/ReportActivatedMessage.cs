using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class ReportActivatedMessage
    {
        public XtraReport MyReport { get; set; }

        public ReportActivatedMessage(XtraReport myReport)
        {
            MyReport = myReport;
        }
    }
}