using Community.VisualStudio.Toolkit;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.startFeature)]
    internal sealed class GitFlowStartFeatureCommand : BaseCommand<GitFlowStartFeatureCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");

            if (string.IsNullOrEmpty(featureName))
            {
                return;
            }

            var flowOptions = await GitHelper.GetGitConfig();
            var options = ProcessHelper.GetOptions(Package);

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand($"checkout {flowOptions.DevelopBranch}") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"checkout -b {flowOptions.FeaturePrefix}{featureName} {flowOptions.DevelopBranch}", false),
                $"Starting feature {featureName}"
            );
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            Command.Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitFlow();
        }
    }
}
