using System.Linq;
using Caliburn.Micro;
using NLog;
using NLog.Targets;

namespace GeniusCode.XtraReports.Designer.Support
{
    [Target("MessagePublishingTarget")]
    public sealed class MessagePublishingTarget : TargetWithLayout
    {       
        protected override void Write(LogEventInfo logEvent)
        {
            Program.DefaultEventAggregator.Publish(new NLogMessage(logEvent));
        }
    }
}
