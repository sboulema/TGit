using System.Linq;

namespace SamirBoulema.TGit.Models
{
    public class GitConfig
    {
        public string MasterBranch = string.Empty;
        public string DevelopBranch = string.Empty;
        public string FeaturePrefix = string.Empty;
        public string ReleasePrefix = string.Empty;
        public string HotfixPrefix = string.Empty;
        public string TagPrefix = string.Empty;
        public string BugTraqMessage = string.Empty;
        public string SvnUrl = string.Empty;

        public GitConfig()
        {
            
        }

        public GitConfig(string input)
        {
            foreach (var line in input.Split(';'))
            {
                if (line.StartsWith("gitflow.branch.master"))
                {
                    MasterBranch = line.Split(' ').Last();
                }
                else if (line.StartsWith("gitflow.branch.develop"))
                {
                    DevelopBranch = line.Split(' ').Last();
                }
                else if (line.StartsWith("gitflow.prefix.feature"))
                {
                    FeaturePrefix = line.Split(' ').Last();
                }
                else if (line.StartsWith("gitflow.prefix.release"))
                {
                    ReleasePrefix = line.Split(' ').Last();
                }
                else if (line.StartsWith("gitflow.prefix.hotfix"))
                {
                    HotfixPrefix = line.Split(' ').Last();
                }
                else if (line.StartsWith("gitflow.prefix.versiontag"))
                {
                    TagPrefix = line.Split(' ').Last();
                }
                else if (line.StartsWith("bugtraq.message"))
                {
                    BugTraqMessage = line.Split(' ').Last();
                }
                else if (line.StartsWith("svn-remote.svn.url"))
                {
                    SvnUrl = line.Split('=').Last();
                }
            }
        }
    }
}
