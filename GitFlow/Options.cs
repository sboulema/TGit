using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SamirBoulema.TGIT
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage
    {
        private string _developBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Develop branch prefix")]
        [Description("Prefix for your Gitflow develop branch")]
        public string DevelopBranch
        {
            get
            {
                return string.IsNullOrEmpty(_developBranch) ? "develop" : _developBranch;
            }
            set { _developBranch = value; }
        }

        private string _featureBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Feature branches prefix")]
        [Description("Prefix for your Gitflow feature branches")]
        public string FeatureBranch
        {
            get
            {
                return string.IsNullOrEmpty(_featureBranch) ? "feature" : _featureBranch;
            }
            set { _featureBranch = value; }
        }

        private string _releaseBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Release branches prefix")]
        [Description("Prefix for your Gitflow release branches")]
        public string ReleaseBranch
        {
            get
            {
                return string.IsNullOrEmpty(_releaseBranch) ? "release" : _releaseBranch;
            }
            set { _releaseBranch = value; }
        }

        private string _masterBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Master branches prefix")]
        [Description("Prefix for your Gitflow master branch")]
        public string MasterBranch
        {
            get
            {
                return string.IsNullOrEmpty(_masterBranch) ? "master" : _masterBranch;
            }
            set { _masterBranch = value; }
        }

        private string _hotfixBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Hotfix branches prefix")]
        [Description("Prefix for your Gitflow hotfix branch")]
        public string HotfixBranch
        {
            get
            {
                return string.IsNullOrEmpty(_hotfixBranch) ? "hotfix" : _hotfixBranch;
            }
            set { _hotfixBranch = value; }
        }

        private string _commitMessage { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Default commit message")]
        [Description("$(BranchName), $(FeatureName), https://msdn.microsoft.com/en-us/library/c02as0cs.aspx")]
        public string CommitMessage
        {
            get
            {
                return _commitMessage == null ? "$(FeatureName)" : _commitMessage;
            }
            set { _commitMessage = value; }
        }
    }
}
