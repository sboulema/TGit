using Community.VisualStudio.Toolkit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.startHotfix)]
    internal sealed class GitFlowStartHotfixCommand : BaseCommand<GitFlowStartHotfixCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var hotfixVersion = Interaction.InputBox("Hotfix Version:", "Start New Hotfix");

            if (string.IsNullOrEmpty(hotfixVersion))
            {
                return;
            }

            var flowOptions = await GitHelper.GetGitConfig();
            var options = await General.GetLiveInstanceAsync();

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new hotfix branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {flowOptions.MasterBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"checkout -b {flowOptions.HotfixPrefix}{hotfixVersion} {flowOptions.MasterBranch}", false),
                $"Starting hotfix {hotfixVersion}"
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
        }
    }
}
