using SamirBoulema.TGit.Helpers;
using SamirBoulema.TGit.Models;
using System;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public partial class FlowDialog : Form
    {
        public GitConfig GitConfig;

        public FlowDialog()
        {
            InitializeComponent();

            Load += FlowDialog_Load;
        }

        private async void FlowDialog_Load(object sender, EventArgs e)
        {
            var gitConfig = await GitHelper.GetGitConfig();

            developTextBox.Text = string.IsNullOrEmpty(gitConfig.DevelopBranch) ? "develop" : gitConfig.DevelopBranch;
            masterTextBox.Text = string.IsNullOrEmpty(gitConfig.MasterBranch) ? "master" : gitConfig.MasterBranch;
            featureTextBox.Text = string.IsNullOrEmpty(gitConfig.FeaturePrefix) ? "feature/" : gitConfig.FeaturePrefix;
            hotfixTextBox.Text = string.IsNullOrEmpty(gitConfig.HotfixPrefix) ? "hotfix/" : gitConfig.HotfixPrefix;
            releaseTextBox.Text = string.IsNullOrEmpty(gitConfig.ReleasePrefix) ? "release/" : gitConfig.ReleasePrefix;
            tagTextBox.Text = gitConfig.TagPrefix;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            GitConfig = new GitConfig
            {
                DevelopBranch = developTextBox.Text,
                MasterBranch = masterTextBox.Text,
                FeaturePrefix = featureTextBox.Text,
                HotfixPrefix = hotfixTextBox.Text,
                ReleasePrefix = releaseTextBox.Text,
                TagPrefix = tagTextBox.Text
            };
            Close();
        }
    }
}
