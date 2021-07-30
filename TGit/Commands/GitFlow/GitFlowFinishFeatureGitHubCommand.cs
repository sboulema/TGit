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

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = await GitHelper.GetGitConfig();
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
            Command.Enabled = await FileHelper.HasSolutionDir() && (await GitHelper.GetCurrentBranchName(false)).StartsWith(gitConfig.FeaturePrefix);
        }
    }
}
