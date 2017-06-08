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
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
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
