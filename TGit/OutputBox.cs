using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public sealed partial class OutputBox : Form
    {
        private readonly string _branchName;
        private readonly string _pushCommand;

        public OutputBox(string branchName, string pushCommand)
        {
            InitializeComponent();

            _branchName = branchName;
            _pushCommand = pushCommand;

            Load += OutputBox_Load;
            richTextBox.TextChanged += TextBox_TextChanged;
        }

        private async void OutputBox_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_branchName))
            {
                localBranchCheckBox.Visible = false;
                remoteBranchCheckBox.Visible = false;
                pushCheckBox.Visible = false;
                richTextBox.Height += 30;
            }
            else
            {
                remoteBranchCheckBox.Enabled = await GitHelper.RemoteBranchExists(_branchName);
            }

            var options = await General.GetLiveInstanceAsync();
            localBranchCheckBox.Checked = options.DeleteLocalBranch;
            remoteBranchCheckBox.Checked = options.DeleteRemoteBranch;
            pushCheckBox.Checked = options.PushChanges;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (localBranchCheckBox.Checked || remoteBranchCheckBox.Checked || pushCheckBox.Checked)
            {
                okButton.Enabled = false;

                var process = ProcessHelper.StartProcessGui(
                     "cmd.exe",
                     $"/c cd \"{FileHelper.GetSolutionDir().Result}\" && " +
                         GitHelper.GetSshSetup().Result +
                         "echo. && " +   
                         (pushCheckBox.Checked ? _pushCommand : "echo.") + 
                         GitHelper.FormatCliCommand($"branch -d {_branchName}") +
                         (remoteBranchCheckBox.Checked ? GitHelper.FormatCliCommand($"push origin --delete {_branchName}", false) : "echo."),
                     string.Empty, string.Empty, this
                );

                okButton.Click -= OkButton_Click;
                okButton.Click += OkButton_Click_Close;
            }
            else
            {
                OkButton_Click_Close(null, null);
            }         
        }

        private void OkButton_Click_Close(object sender, EventArgs e)
        {
            Close();
            richTextBox.Clear();
            okButton.Enabled = false;
        }

        private void ResolveButton_Click(object sender, EventArgs e)
        {
            OkButton_Click(null, null);
            ProcessHelper.RunTortoiseGitCommand("resolve").FireAndForget();
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            OkButton_Click(null, null);
            ProcessHelper.RunTortoiseGitCommand("repostatus").FireAndForget();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
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
