using System;
using System.Windows.Forms;

namespace FundaRealEstateBV.TGIT
{
    public sealed partial class OutputBox : Form
    {
        public OutputBox()
        {
            InitializeComponent();
        }

        public OutputBox(string title)
        {
            InitializeComponent();
            Text = title;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            textBox.Clear();
            okButton.Enabled = false;
        }
    }
}
