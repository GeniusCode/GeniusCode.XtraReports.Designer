using System.Linq;
using System.Windows.Forms;
using GeniusCode.XtraReports.Designer.Messaging;

namespace GeniusCode.XtraReports.Designer.Popups
{
    public partial class ShowMessages : Form
    {
        public ShowMessages()
        {
            InitializeComponent();
        }

        public ShowMessages(DebugMessageHandler debugDebugMessageHandler)
        {
            InitializeComponent();
            this.messageInfoBindingSource.DataSource = debugDebugMessageHandler.GetMessageInfos();
        }
    }
}
