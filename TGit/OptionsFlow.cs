using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using SamirBoulema.TGit.Helpers;

// ReSharper disable InconsistentNaming

namespace SamirBoulema.TGit
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionFlowPageGrid : DialogPage
    {
        private ProcessHelper _processHelper;

        public void SetProcessHelper(ProcessHelper processHelper)
        {
            _processHelper = processHelper;
        }

        private string _developBranch { get; set; }
        [Category("TGit")]
        [DisplayName(@"Develop branch name")]
        [Description("The name of the branch treated as develop")]
        public string DevelopBranch
        {
            get
            {
                var devBranch = _processHelper.StartProcessGitResult("config --get gitflow.branch.develop");
                if (!string.IsNullOrEmpty(devBranch))
                {
                    return devBranch;
                }
                return string.IsNullOrEmpty(_developBranch) ? "develop" : _developBranch;
            }
            set { _developBranch = value; }
        }

        private string _featurePrefix { get; set; }
        [Category("TGit")]
        [DisplayName(@"Feature branches prefix")]
        [Description("The prefix for feature branches")]
        public string FeaturePrefix
        {
            get
            {
                var featurePrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.feature");
                if (!string.IsNullOrEmpty(featurePrefix))
                {
                    return featurePrefix;
                }
                return string.IsNullOrEmpty(_featurePrefix) ? "feature/" : _featurePrefix;
            }
            set { _featurePrefix = value; }
        }

        private string _releasePrefix { get; set; }
        [Category("TGit")]
        [DisplayName(@"Release branches prefix")]
        [Description("The prefix for release branches")]
        public string ReleasePrefix
        {
            get
            {
                var releasePrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.release");
                if (!string.IsNullOrEmpty(releasePrefix))
                {
                    return releasePrefix;
                }
                return string.IsNullOrEmpty(_releasePrefix) ? "release/" : _releasePrefix;
            }
            set { _releasePrefix = value; }
        }

        private string _masterBranch { get; set; }
        [Category("TGit")]
        [DisplayName(@"Master branch name")]
        [Description("The name of the branch treated as master")]
        public string MasterBranch
        {
            get
            {
                var masterBranch = _processHelper.StartProcessGitResult("config --get gitflow.branch.master");
                if (!string.IsNullOrEmpty(masterBranch))
                {
                    return masterBranch;
                }
                return string.IsNullOrEmpty(_masterBranch) ? "master" : _masterBranch;
            }
            set { _masterBranch = value; }
        }

        private string _hotfixPrefix { get; set; }
        [Category("TGit")]
        [DisplayName(@"Hotfix branches prefix")]
        [Description("The prefix for hotfix branches")]
        public string HotfixPrefix
        {
            get
            {
                var hotfixPrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.hotfix");
                if (!string.IsNullOrEmpty(hotfixPrefix))
                {
                    return hotfixPrefix;
                }
                return string.IsNullOrEmpty(_hotfixPrefix) ? "hotfix/" : _hotfixPrefix;
            }
            set { _hotfixPrefix = value; }
        }
    }
}
