using System.Linq;
using NLog;

namespace GeniusCode.XtraReports.Designer.Support
{
    public class NLogMessage
    {
        public LogEventInfo LogMessage { get; set; }

        public NLogMessage(LogEventInfo logMessage)
        {
            LogMessage = logMessage;
        }
    }
}