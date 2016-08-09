using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SamirBoulema.TGit
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage
    {
        private string _commitMessage { get; set; }
        [Category("TGit")]
        [DisplayName(@"Default commit message")]
        [Description("$(BranchName), $(FeatureName), https://msdn.microsoft.com/en-us/library/c02as0cs.aspx")]
        public string CommitMessage
        {
            get
            {
                return _commitMessage ?? string.Empty;
            }
            set { _commitMessage = value; }
        }

        [Category("TGit")]
        [DisplayName(@"Close dialog after operation")]
        [Description("0: Close manually, 1: Auto-close if no further options are available, 2: Auto-close if no errors")]
        public int CloseOnEnd { get; set; }
    }
}
