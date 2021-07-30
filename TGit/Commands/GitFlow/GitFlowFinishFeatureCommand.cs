using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.finishFeature)]
    internal sealed class GitFlowFinishFeatureCommand : BaseCommand<GitFlowFinishFeatureCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var featureBranch = await GitHelper.GetCurrentBranchName(false);
            var featureName = await GitHelper.GetCurrentBranchName(true);
            var gitConfig = await GitHelper.GetGitConfig();
            var options = ProcessHelper.GetOptions(Package);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Push all changes to develop
             * 5. Delete the local feature branch
             * 6. Delete the remote feature branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {featureBranch}", false),
                $"Finishing feature {featureName}",
                featureBranch, null, options, GitHelper.FormatCliCommand($"push origin {gitConfig.DevelopBranch}")
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = await GitHelper.GetGitConfig();
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
            Command.Enabled = await FileHelper.HasSolutionDir() && (await GitHelper.GetCurrentBranchName(false)).StartsWith(gitConfig.FeaturePrefix);
        }
    }
}
