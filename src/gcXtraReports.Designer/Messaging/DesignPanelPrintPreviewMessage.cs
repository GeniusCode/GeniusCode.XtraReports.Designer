using System.Linq;
using DevExpress.XtraReports.UserDesigner;

namespace GeniusCode.XtraReports.Designer.Messaging
{
    public class DesignPanelPrintPreviewMessage
    {
        public XRDesignPanel DesignPanel { get; set; }

        public DesignPanelPrintPreviewMessage(XRDesignPanel designPanel)
        {
            DesignPanel = designPanel;
        }
    }
}