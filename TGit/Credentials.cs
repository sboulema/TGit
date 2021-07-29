using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System;
using System.Threading.Tasks;
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
            ProcessHelper.StartProcessGit($"config user.name \"{nameTextBox.Text}\"").FireAndForget();
            ProcessHelper.StartProcessGit($"config user.email \"{emailTextBox.Text}\"").FireAndForget();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
