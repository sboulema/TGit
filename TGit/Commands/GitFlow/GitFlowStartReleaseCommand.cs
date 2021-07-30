using Community.VisualStudio.Toolkit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.startRelease)]
    internal sealed class GitFlowStartReleaseCommand : BaseCommand<GitFlowStartReleaseCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");

            if (string.IsNullOrEmpty(releaseVersion))
            {
                return;
            }

            var flowOptions = await GitHelper.GetGitConfig();
            var options = await General.GetLiveInstanceAsync();

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"checkout -b {flowOptions.ReleasePrefix}{releaseVersion} {flowOptions.DevelopBranch}", false),
                $"Starting release {releaseVersion}"
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
        }
    }
}
