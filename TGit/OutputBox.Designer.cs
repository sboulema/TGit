namespace SamirBoulema.TGit
{
    sealed partial class OutputBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputBox));
            this.okButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.localBranchCheckBox = new System.Windows.Forms.CheckBox();
            this.remoteBranchCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(272, 3);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(141, 290);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(347, 32);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox.Location = new System.Drawing.Point(12, 12);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(476, 263);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            // 
            // localBranchCheckBox
            // 
            this.localBranchCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.localBranchCheckBox.AutoSize = true;
            this.localBranchCheckBox.Location = new System.Drawing.Point(12, 281);
            this.localBranchCheckBox.Name = "localBranchCheckBox";
            this.localBranchCheckBox.Size = new System.Drawing.Size(118, 17);
            this.localBranchCheckBox.TabIndex = 5;
            this.localBranchCheckBox.Text = "Delete local branch";
            this.localBranchCheckBox.UseVisualStyleBackColor = true;
            // 
            // remoteBranchCheckBox
            // 
            this.remoteBranchCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.remoteBranchCheckBox.AutoSize = true;
            this.remoteBranchCheckBox.Location = new System.Drawing.Point(12, 305);
            this.remoteBranchCheckBox.Name = "remoteBranchCheckBox";
            this.remoteBranchCheckBox.Size = new System.Drawing.Size(128, 17);
            this.remoteBranchCheckBox.TabIndex = 6;
            this.remoteBranchCheckBox.Text = "Delete remote branch";
            this.remoteBranchCheckBox.UseVisualStyleBackColor = true;
            // 
            // OutputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 334);
            this.Controls.Add(this.remoteBranchCheckBox);
            this.Controls.Add(this.localBranchCheckBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.richTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TGit";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button okButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.CheckBox localBranchCheckBox;
        private System.Windows.Forms.CheckBox remoteBranchCheckBox;
    }
}