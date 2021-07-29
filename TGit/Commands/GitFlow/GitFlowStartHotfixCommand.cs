using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.StartHotfix)]
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
            var options = ProcessHelper.GetOptions(Package);

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

        protected override void BeforeQueryStatus(System.EventArgs e)
        {
            Command.Visible = FileHelper.HasSolutionDir().Result && GitHelper.IsGitFlow().Result;
        }
    }
}
