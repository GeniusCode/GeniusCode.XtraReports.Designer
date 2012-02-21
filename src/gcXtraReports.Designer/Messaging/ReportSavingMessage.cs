using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class ReportSavingMessage
    {
        public XtraReport Report { get; private set; }
        public string Url { get; private set; }

        public ReportSavingMessage(XtraReport report, string url)
        {
            Url = url;
            Report = report;
        }
    }
}
