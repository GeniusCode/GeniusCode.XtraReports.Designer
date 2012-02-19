using System.Linq;
using DevExpress.XtraReports.UI;
using GeniusCode.XtraReports.Design;
using GeniusCode.XtraReports.Runtime;
using GeniusCode.XtraReports.Runtime.Actions;
using GeniusCode.XtraReports.Runtime.Support;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class ReportControllerFactory : IReportControllerFactory
    {
        private readonly IReportControlActionFacade _xrRuntimeActionFacade;

        public ReportControllerFactory() : this(null)
        {
        }

        public ReportControllerFactory(IReportControlActionFacade xrRuntimeActionFacade)
        {
            _xrRuntimeActionFacade = xrRuntimeActionFacade;
        }

        public IReportController GetController(XtraReport report)
        {
            return new ReportController(report,_xrRuntimeActionFacade);
        }
    }
}