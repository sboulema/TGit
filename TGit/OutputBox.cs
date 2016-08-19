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
        private readonly DTE _dte;
        private readonly ProcessHelper _processHelper;
        private readonly GitHelper _gitHelper;
        private readonly string _branchName;

        public OutputBox(DTE dte, string branchName, OptionPageGrid options)
        {
            InitializeComponent();
            _dte = dte;
            _processHelper = new ProcessHelper(dte);
            _gitHelper = new GitHelper(_processHelper);
            _branchName = branchName;

            richTextBox.TextChanged += textBox_TextChanged;

            if (string.IsNullOrEmpty(branchName))
            {
                localBranchCheckBox.Visible = false;
                remoteBranchCheckBox.Visible = false;
            }
            else
            {
                remoteBranchCheckBox.Enabled = new GitHelper(_processHelper).RemoteBranchExists(branchName);
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
                var process = _processHelper.StartProcessGui(
                     "cmd.exe",
                     $"/c cd \"{FileHelper.GetSolutionDir(_dte)}\" && " +
                         _gitHelper.GetSshSetup() +
                         FormatCliCommand($"branch -d {_branchName}") +
                         (remoteBranchCheckBox.Checked ? FormatCliCommand($"push origin --delete {_branchName}", false) : "echo."),
                     "Deleting branches...",
                     string.Empty, this
                 );
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Close();
                    richTextBox.Clear();
                    okButton.Enabled = false;
                }
            }
            else
            {
                Close();
                richTextBox.Clear();
                okButton.Enabled = false;
            }         
        }

        private static string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            return $"echo ^> {Path.GetFileNameWithoutExtension(FileHelper.GetMSysGit())} {gitCommand} && \"{FileHelper.GetMSysGit()}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        private void ResolveButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            var solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:resolve /path:\"{solutionDir}\"");
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            var solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{solutionDir}\"");
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
