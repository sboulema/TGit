using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SamirBoulema.TGit.Helpers;
using static System.Windows.Forms.Design.AxImporter;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.FinishHotfix)]
    internal sealed class GitFlowFinishHotfixCommand : BaseCommand<GitFlowFinishHotfixCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var hotfixBranch = await GitHelper.GetCurrentBranchName(false);
            var hotfixName = await GitHelper.GetCurrentBranchName(true);
            var gitConfig = await GitHelper.GetGitConfig();
            var options = ProcessHelper.GetOptions(Package);

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
                hotfixBranch, null, options,
                    GitHelper.FormatCliCommand($"push origin {gitConfig.DevelopBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.MasterBranch}") +
                    GitHelper.FormatCliCommand($"push origin {gitConfig.TagPrefix}{hotfixName}"),
                    Package
            );
        }

        protected override void BeforeQueryStatus(System.EventArgs e)
        {
            var gitConfig = GitHelper.GetGitConfig().Result;
            Command.Visible = FileHelper.HasSolutionDir().Result && GitHelper.IsGitFlow().Result;
            Command.Enabled = FileHelper.HasSolutionDir().Result && GitHelper.GetCurrentBranchName(false).Result.StartsWith(gitConfig.HotfixPrefix);
        }
    }
}
