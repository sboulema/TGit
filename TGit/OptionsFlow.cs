using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace SamirBoulema.TGit
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionFlowPageGrid : DialogPage
    {
        private string _developBranch { get; set; }
        [Category("TGit")]
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
        [Category("TGit")]
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
        [Category("TGit")]
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
        [Category("TGit")]
        [DisplayName(@"Master branch prefix")]
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
        [Category("TGit")]
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
    }
}
