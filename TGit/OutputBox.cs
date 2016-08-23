using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public sealed partial class OutputBox : Form
    {
        private readonly string _branchName;

        public OutputBox(string branchName, OptionPageGrid options)
        {
            InitializeComponent();
            _branchName = branchName;

            richTextBox.TextChanged += textBox_TextChanged;

            if (string.IsNullOrEmpty(branchName))
            {
                localBranchCheckBox.Visible = false;
                remoteBranchCheckBox.Visible = false;
            }
            else
            {
                remoteBranchCheckBox.Enabled = GitHelper.RemoteBranchExists(branchName);
            }

            if (options != null)
            {
                localBranchCheckBox.Checked = options.DeleteLocalBranch;
                remoteBranchCheckBox.Checked = options.DeleteRemoteBranch;
            }           
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (localBranchCheckBox.Checked || remoteBranchCheckBox.Checked)
            {
                var process = ProcessHelper.StartProcessGui(
                     "cmd.exe",
                     $"/c cd \"{EnvHelper.SolutionDir}\" && " +
                         GitHelper.GetSshSetup() +
                         FormatCliCommand($"branch -d {_branchName}") +
                         (remoteBranchCheckBox.Checked ? FormatCliCommand($"push origin --delete {_branchName}", false) : "echo."),
                     "Deleting branches...", string.Empty, this
                 );
                process.WaitForExit();
                okButton.Click += OkButton_Click_Close;
            }
            else
            {
                Close();
                richTextBox.Clear();
                okButton.Enabled = false;
            }         
        }

        private void OkButton_Click_Close(object sender, EventArgs e)
        {
            Close();
            richTextBox.Clear();
            okButton.Enabled = false;
        }

        private static string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            return $"echo ^> {Path.GetFileNameWithoutExtension(FileHelper.GetMSysGit())} {gitCommand} && \"{FileHelper.GetMSysGit()}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void ResolveButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            ProcessHelper.StartTortoiseGitProc($"/command:resolve /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            if (string.IsNullOrEmpty(EnvHelper.SolutionDir)) return;
            ProcessHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!richTextBox.Text.ToLower().Contains("error")) return;

            if (richTextBox.Text.ToLower().Contains("fix conflicts") && !flowLayoutPanel1.Controls.Find("Resolve", true).Any())
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

            if (richTextBox.Text.ToLower().Contains("stash") && !flowLayoutPanel1.Controls.Find("Stash", true).Any())
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
