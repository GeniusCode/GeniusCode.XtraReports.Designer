using System;
using System.Linq;
using System.Windows.Forms;
using GeniusCode.XtraReports.Designer.Support;

namespace GeniusCode.XtraReports.Designer.Popups
{
    public partial class ChooseProject : Form
    {
        private readonly AppBootStrapper _bootStrapper;

        public ChooseProject()
        {
            InitializeComponent();
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("VS2010");
        }

        private void acceptAndContinueBarButtonItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var item = multipleProjectsListBoxControl.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(item))
            {
                MessageBox.Show("Please retry", "Item was not selected, please try again");
                return;
            }

            try
            {
                _bootStrapper.SetProjectName(item);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "An error has happened: \r\n" + ex.Message, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

            }
            

        }

        public ChooseProject(AppBootStrapper bootStrapper) : this()
        {
            _bootStrapper = bootStrapper;

            this.multipleProjectsListBoxControl.DataSource = _bootStrapper.GetProjects().ToArray();

        }


    }
}
