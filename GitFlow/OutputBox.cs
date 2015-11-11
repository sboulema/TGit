using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SamirBoulema.TGIT
{
    public sealed partial class OutputBox : Form
    {
        private DTE dte;

        public OutputBox(DTE dte)
        {
            InitializeComponent();
            this.dte = dte;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            textBox.Clear();
            okButton.Enabled = false;
        }

        private void ResolveButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            string solutionDir = new FileHelper(dte).GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            new ProcessHelper(dte).StartTortoiseGitProc(string.Format("/command:resolve /path:\"{0}\"", solutionDir));
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            string solutionDir = new FileHelper(dte).GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            new ProcessHelper(dte).StartTortoiseGitProc(string.Format("/command:repostatus /path:\"{0}\"", solutionDir));
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!textBox.Text.ToLower().Contains("error")) return;

            if (textBox.Text.ToLower().Contains("fix conflicts") && !flowLayoutPanel1.Controls.Find("Resolve", true).Any())
            {
                var resolveButton = new Button();
                resolveButton.Text = "Resolve";
                resolveButton.Name = "Resolve";
                resolveButton.AutoSize = true;
                resolveButton.Click += ResolveButton_Click;
                flowLayoutPanel1.Controls.Add(resolveButton);
            }

            if (textBox.Text.ToLower().Contains("stash") && !flowLayoutPanel1.Controls.Find("Stash", true).Any())
            {
                var stashButton = new Button();
                stashButton.Text = "Show changes";
                stashButton.Name = "Stash";
                stashButton.AutoSize = true;
                stashButton.Click += StashButton_Click;
                flowLayoutPanel1.Controls.Add(stashButton);
            }
        }
    }
}
