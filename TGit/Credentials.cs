using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public partial class Credentials : Form
    {
        private readonly ProcessHelper _processHelper;

        public Credentials(DTE dte)
        {
            InitializeComponent();
            _processHelper = new ProcessHelper(dte);
            emailTextBox.KeyDown += EmailTextBox_KeyDown;
        }

        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                okButton_Click(null, null);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _processHelper.StartProcessGit(string.Format("config user.name \"{0}\"", nameTextBox.Text));
            _processHelper.StartProcessGit(string.Format("config user.email \"{0}\"", emailTextBox.Text));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
