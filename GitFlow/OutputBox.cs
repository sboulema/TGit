using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SamirBoulema.TGIT
{
    public sealed partial class OutputBox : Form
    {
        private readonly FileHelper _fileHelper;
        private readonly ProcessHelper _processHelper;

        public OutputBox(DTE dte)
        {
            InitializeComponent();
            _fileHelper = new FileHelper(dte);
            _processHelper = new ProcessHelper(dte);
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
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:resolve /path:\"{solutionDir}\"");
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            var solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{solutionDir}\"");
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!textBox.Text.ToLower().Contains("error")) return;

            if (textBox.Text.ToLower().Contains("fix conflicts") && !flowLayoutPanel1.Controls.Find("Resolve", true).Any())
            {
                var resolveButton = new Button
                {
                    Text = "Resolve",
                    Name = "Resolve",
                    AutoSize = true
                };
                resolveButton.Click += ResolveButton_Click;
                flowLayoutPanel1.Controls.Add(resolveButton);
            }

            if (textBox.Text.ToLower().Contains("stash") && !flowLayoutPanel1.Controls.Find("Stash", true).Any())
            {
                var stashButton = new Button
                {
                    Text = "Show changes",
                    Name = "Stash",
                    AutoSize = true
                };
                stashButton.Click += StashButton_Click;
                flowLayoutPanel1.Controls.Add(stashButton);
            }
        }
    }
}
