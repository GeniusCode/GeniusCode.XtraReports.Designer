using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UserDesigner;
using GeniusCode.XtraReports.Designer.Popups;

namespace GeniusCode.XtraReports.Designer
{
    public static class Extensions
    {
        public static void RedrawFieldListOnDesignPanel(this MessagingDesignForm form, XRDesignPanel designPanel)
        {
            if (form.ActiveDesignPanel == null) return;

            designPanel = designPanel ?? form.ActiveDesignPanel;

            // Update the Field List.
            var fieldList = (FieldListDockPanel)form.DesignDockManager[DesignDockPanelType.FieldList];
            var host = (IDesignerHost)designPanel.GetService(typeof(IDesignerHost));
            fieldList.UpdateDataSource(host);
        }
    }
}
