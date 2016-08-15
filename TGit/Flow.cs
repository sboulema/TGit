using System;
using System.Windows.Forms;

namespace SamirBoulema.TGit
{
    public partial class Flow : Form
    {
        public FlowOptions FlowOptions;

        public Flow()
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
            FlowOptions = new FlowOptions
            {
                DevelopBranch = developTextBox.Text,
                MasterBranch = masterTextBox.Text,
                FeaturePrefix = featureTextBox.Text,
                HotfixPrefix = hotfixTextBox.Text,
                ReleasePrefix = releaseTextBox.Text
            };
            Close();
        }
    }
}
