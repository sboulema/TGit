using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public sealed partial class OutputBox : Form
    {
        private readonly AsyncPackage _package;
        private readonly string _branchName;
        private readonly string _pushCommand;

        public OutputBox(AsyncPackage package, string branchName, OptionPageGrid options, string pushCommand)
        {
            InitializeComponent();

            _package = package;
            _branchName = branchName;
            _pushCommand = pushCommand;

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
                remoteBranchCheckBox.Enabled = GitHelper.RemoteBranchExists(branchName).Result;
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

                okButton.Click -= okButton_Click;
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
            okButton_Click(null, null);
            ProcessHelper.RunTortoiseGitCommand(_package, "resolve").FireAndForget();
        }

        private void StashButton_Click(object sender, EventArgs e)
        {
            okButton_Click(null, null);
            ProcessHelper.RunTortoiseGitCommand(_package, "repostatus").FireAndForget();
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
