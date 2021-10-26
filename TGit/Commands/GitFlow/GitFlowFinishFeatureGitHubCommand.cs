using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.finishFeatureGitHub)]
    internal sealed class GitFlowFinishFeatureGitHubCommand : BaseCommand<GitFlowFinishFeatureGitHubCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var featureBranch = await GitHelper.GetCurrentBranchName(false);
            var featureName = await GitHelper.GetCurrentBranchName(true);
            var options = await General.GetLiveInstanceAsync();
            var gitConfig = await GitHelper.GetGitConfig();
            var isGitFlow = GitHelper.IsGitFlow(gitConfig);

            if (!isGitFlow)
            {
                await VS.MessageBox.ShowErrorAsync("GitFlow is not initialized.");
                return;
            }

            if (!featureBranch.StartsWith(gitConfig.FeaturePrefix))
            {
                await VS.MessageBox.ShowErrorAsync("Current branch is not a feature branch.");
                return;
            }

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
                featureBranch, null, GitHelper.FormatCliCommand("push origin master"));
        }
    }
}
