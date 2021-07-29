using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.FinishFeatureGitHub)]
    internal sealed class GitFlowFinishFeatureGitHubCommand : BaseCommand<GitFlowFinishFeatureGitHubCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var featureBranch = await GitHelper.GetCurrentBranchName(false);
            var featureName = await GitHelper.GetCurrentBranchName(true);
            var options = ProcessHelper.GetOptions(Package);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the feature branch to master
             * 4. Push all changes to master
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand("checkout master") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {featureBranch}", false),
                $"Finishing feature {featureName}",
                featureBranch, null, options, GitHelper.FormatCliCommand("push origin master"));
        }

        protected override void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = GitHelper.GetGitConfig().Result;
            Command.Visible = FileHelper.HasSolutionDir().Result && GitHelper.IsGitFlow().Result;
            Command.Enabled = FileHelper.HasSolutionDir().Result && GitHelper.GetCurrentBranchName(false).Result.StartsWith(gitConfig.FeaturePrefix);
        }
    }
}
