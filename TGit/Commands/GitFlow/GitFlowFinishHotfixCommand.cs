using Community.VisualStudio.Toolkit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.finishHotfix)]
    internal sealed class GitFlowFinishHotfixCommand : BaseCommand<GitFlowFinishHotfixCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var hotfixBranch = await GitHelper.GetCurrentBranchName(false);
            var hotfixName = await GitHelper.GetCurrentBranchName(true);
            var gitConfig = await GitHelper.GetGitConfig();
            var options = await General.GetLiveInstanceAsync();

            var tagMessage = string.Empty;

            if (options.UseAnnotatedTag)
            {
                tagMessage = Interaction.InputBox("Tag message:", "Finish hotfix");
            }

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the hotfix branch to master
             * 4. Tag the hotfix
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the hotfix branch to develop
             * 8. Push all changes to develop
             * 9. Push all changes to master
             * 10. Push the tag
             * 11. Delete the local hotfix branch
             * 12. Delete the remote hotfix branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {gitConfig.MasterBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {hotfixBranch}") +
                    (options.UseAnnotatedTag ? GitHelper.FormatCliCommand($"tag -a {gitConfig.TagPrefix}{hotfixName} -m \"{tagMessage}\"") : GitHelper.FormatCliCommand($"tag {gitConfig.TagPrefix}{hotfixName}")) +
                    GitHelper.FormatCliCommand($"checkout {gitConfig.DevelopBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"merge --no-ff {hotfixBranch}", false),
                $"Finishing hotfix {hotfixName}",
                hotfixBranch, null,
                    GitHelper.FormatCliCommand($"push origin {gitConfig.DevelopBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.MasterBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.TagPrefix}{hotfixName}")
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = await GitHelper.GetGitConfig();
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
            Command.Enabled = await FileHelper.HasSolutionDir() && (await GitHelper.GetCurrentBranchName(false)).StartsWith(gitConfig.HotfixPrefix);
        }
    }
}
