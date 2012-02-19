using System.Linq;
using System.Windows.Forms;
using GeniusCode.XtraReports.Designer.Support;

namespace GeniusCode.XtraReports.Designer.Popups
{
    public partial class NoProjectsExistWarning : Form
    {
        private readonly AppBootStrapper _bootStrapper;

        public NoProjectsExistWarning()
        {
            InitializeComponent();
        }

        public NoProjectsExistWarning(AppBootStrapper bootStrapper) : this()
        {
            _bootStrapper = bootStrapper;

            memoEdit1.Text = memoEdit1.Text.Replace("@PATH", bootStrapper.RootPath);
        }
    }
}
