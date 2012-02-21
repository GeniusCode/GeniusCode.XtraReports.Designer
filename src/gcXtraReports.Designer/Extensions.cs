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
        public static void RedrawFieldListOnActiveDesignPanel(this MessagingDesignForm form)
        {
            if (form.ActiveDesignPanel == null) return;

            // Update the Field List.
            var fieldList = (FieldListDockPanel)form.DesignDockManager[DesignDockPanelType.FieldList];
            var host = (IDesignerHost)form.ActiveDesignPanel.GetService(typeof(IDesignerHost));
            fieldList.UpdateDataSource(host);
        }
    }
}
