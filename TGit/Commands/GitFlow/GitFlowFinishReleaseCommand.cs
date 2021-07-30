using Community.VisualStudio.Toolkit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.finishRelease)]
    internal sealed class GitFlowFinishReleaseCommand : BaseCommand<GitFlowFinishReleaseCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var releaseBranch = await GitHelper.GetCurrentBranchName(false);
            var releaseName = await GitHelper.GetCurrentBranchName(true);
            var gitConfig = await GitHelper.GetGitConfig();
            var options = await General.GetLiveInstanceAsync();

            var tagMessage = string.Empty;

            if (options.UseAnnotatedTag)
            {
                tagMessage = Interaction.InputBox("Tag message:", "Finish release");
            }

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the release branch to master
             * 4. Tag the release
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the release branch to develop
             * 8. Push all changes to develop
             * 9. Push all changes to master
             * 10. Push the tag
             * 11. Delete the local release branch
             * 12. Delete the remote release branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {gitConfig.MasterBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {releaseBranch}") +
                    (options.UseAnnotatedTag ? GitHelper.FormatCliCommand($"tag -a {gitConfig.TagPrefix}{releaseName} -m \"{tagMessage}\"") : GitHelper.FormatCliCommand($"tag {gitConfig.TagPrefix}{releaseName}")) +
                    GitHelper.FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {releaseBranch}", false),
                $"Finishing release {releaseName}",
                releaseBranch, null,
                    GitHelper.FormatCliCommand($"push origin {gitConfig.DevelopBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.MasterBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.TagPrefix}{releaseName}")
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = await GitHelper.GetGitConfig();
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
            Command.Enabled = await FileHelper.HasSolutionDir() && (await GitHelper.GetCurrentBranchName(false)).StartsWith(gitConfig.ReleasePrefix);
        }
    }
}
