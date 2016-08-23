using SamirBoulema.TGit.Helpers;
using System;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public partial class Credentials : Form
    {
        public Credentials()
        {
            InitializeComponent();
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
            ProcessHelper.StartProcessGit($"config user.name \"{nameTextBox.Text}\"");
            ProcessHelper.StartProcessGit($"config user.email \"{emailTextBox.Text}\"");
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
