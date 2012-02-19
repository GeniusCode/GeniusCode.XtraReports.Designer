using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class ReportActivatedBySubreportMessage
    {
        public gcXtraReport NewReport { get; set; }
        public SubreportBase SelectedSubreport { get; set; }

        public ReportActivatedBySubreportMessage(gcXtraReport myReport, SubreportBase selectedSubreport)
        {
            NewReport = myReport;
            SelectedSubreport = selectedSubreport;
        }
    }
}