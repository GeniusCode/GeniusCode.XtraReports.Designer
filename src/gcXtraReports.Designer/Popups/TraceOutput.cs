using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Caliburn.Micro;
using GeniusCode.XtraReports.Designer.Support;

namespace GeniusCode.XtraReports.Designer.Popups
{
    public partial class TraceOutput : Form, IHandle<NLogMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public TraceOutput()
        {
            InitializeComponent();
        }

        public TraceOutput(IEventAggregator eventAggregator): this()
        {
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public void Handle(NLogMessage message)
        {
            memoEdit1.Text = message.LogMessage.FormattedMessage + "\r\n" + memoEdit1.Text;
            Refresh();
        }

        private void TraceOutput_Load(object sender, EventArgs e)
        {
        }

        private void TraceOutput_FormClosed(object sender, FormClosedEventArgs e)
        {
            _eventAggregator.Unsubscribe(this);
        }

    }
}
