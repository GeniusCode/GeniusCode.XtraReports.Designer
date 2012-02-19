using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Runtime;

namespace GeniusCode.XtraReports.Design
{
    public interface IReportControllerFactory
    {
        IReportController GetController(XtraReport report);
    }

}
