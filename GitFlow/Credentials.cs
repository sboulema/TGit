using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using System;
using System.Windows.Forms;

namespace SamirBoulema.TGIT
{
    public partial class Credentials : Form
    {
        private ProcessHelper processHelper;

        public Credentials(DTE dte)
        {
            InitializeComponent();
            processHelper = new ProcessHelper(dte);
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
            processHelper.StartProcessGit(string.Format("config user.name \"{0}\"", nameTextBox.Text));
            processHelper.StartProcessGit(string.Format("config user.email \"{0}\"", emailTextBox.Text));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
