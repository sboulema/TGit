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
        private readonly string _pushCommand;
        private readonly DTE _dte;
        private readonly EnvHelper _envHelper;

        public OutputBox(string branchName, OptionPageGrid options, string pushCommand, DTE dte, EnvHelper envHelper)
        {
            InitializeComponent();

            _branchName = branchName;
            _pushCommand = pushCommand;
            _dte = dte;
            _envHelper = envHelper;

            richTextBox.TextChanged += textBox_TextChanged;

            if (string.IsNullOrEmpty(branchName))
            {
                localBranchCheckBox.Visible = false;
                remoteBranchCheckBox.Visible = false;
                pushCheckBox.Visible = false;
                richTextBox.Height += 30;
            }
            else
            {
                remoteBranchCheckBox.Enabled = GitHelper.RemoteBranchExists(_envHelper, branchName);
            }

            if (options != null)
            {
                localBranchCheckBox.Checked = options.DeleteLocalBranch;
                remoteBranchCheckBox.Checked = options.DeleteRemoteBranch;
                pushCheckBox.Checked = options.PushChanges;
            }           
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (localBranchCheckBox.Checked || remoteBranchCheckBox.Checked || pushCheckBox.Checked)
            {
                var process = ProcessHelper.StartProcessGui(_dte, _envHelper,
                     "cmd.exe",
                     $"/c cd \"{_envHelper.GetSolutionDir()}\" && " +
                         GitHelper.GetSshSetup(_envHelper) +
                         "echo. && " +   
                         (pushCheckBox.Checked ? _pushCommand : "echo.") + 
                         FormatCliCommand($"branch -d {_branchName}") +
                         (remoteBranchCheckBox.Checked ? FormatCliCommand($"push origin --delete {_branchName}", false) : "echo."),
                     "Finishing...", string.Empty, this
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
            if (string.IsNullOrEmpty(_envHelper.GetSolutionDir())) return;
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:resolve /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            if (string.IsNullOrEmpty(_envHelper.GetSolutionDir())) return;
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repostatus /path:\"{_envHelper.GetSolutionDir()}\"");
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
