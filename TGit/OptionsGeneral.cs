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
        [Category("TGit")]
        [DisplayName(@"Default commit message")]
        [Description("$(BranchName), $(FeatureName), https://msdn.microsoft.com/en-us/library/c02as0cs.aspx")]
        public string CommitMessage { get; set; }

        [Category("TGit")]
        [DisplayName(@"Close dialog after operation")]
        [Description("0: Close manually, 1: Auto-close if no further options are available, 2: Auto-close if no errors")]
        public int CloseOnEnd { get; set; }

        [Category("TGit")]
        [DisplayName(@"Delete local branch")]
        [Description("When finishing a feature delete the local branch by default")]
        public bool DeleteLocalBranch { get; set; }

        [Category("TGit")]
        [DisplayName(@"Delete remote branch")]
        [Description("When finishing a feature delete the remote branch by default")]
        public bool DeleteRemoteBranch { get; set; }

        [Category("TGit")]
        [DisplayName(@"Push changes")]
        [Description("When finishing a feature push the changes to the remote by default")]
        public bool PushChanges { get; set; }

        [Category("TGit")]
        [DisplayName(@"Pull changes")]
        [Description("When starting/finishing a feature pull the changes from the remote by default")]
        public bool PullChanges { get; set; } = true;

        [Category("TGit")]
        [DisplayName(@"Default bug id")]
        [Description("$(BranchName), $(FeatureName), https://msdn.microsoft.com/en-us/library/c02as0cs.aspx")]
        public string BugId { get; set; }
    }
}
