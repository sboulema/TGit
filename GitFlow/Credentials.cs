using EnvDTE;
using FundaRealEstateBV.TGIT.Helpers;
using System;
using System.Windows.Forms;

namespace FundaRealEstateBV.TGIT
{
    public partial class Credentials : Form
    {
        private ProcessHelper _processHelper;

        public Credentials(DTE dte)
        {
            InitializeComponent();
            _processHelper = new ProcessHelper(dte);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _processHelper.StartProcessGit(string.Format("config user.name \"{0}\"", nameTextBox.Text));
            _processHelper.StartProcessGit(string.Format("config user.email \"{0}\"", emailTextBox.Text));
        }
    }
}
