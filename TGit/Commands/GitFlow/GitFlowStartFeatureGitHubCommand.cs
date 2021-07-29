using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.StartFeatureGitHub)]
    internal sealed class GitFlowStartFeatureGitHubCommand : BaseCommand<GitFlowStartFeatureGitHubCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var featureName = Interaction.InputBox("Feature Name:", "Start New Feature");

            if (string.IsNullOrEmpty(featureName))
            {
                return;
            }

            var options = ProcessHelper.GetOptions(Package);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Create and switch to a new branch
             */
            await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                    await GitHelper.GetSshSetup() +
                    GitHelper.FormatCliCommand("checkout master") +
                    (options.PullChanges ? GitHelper.FormatCliCommand("pull") : string.Empty) +
                    GitHelper.FormatCliCommand($"checkout -b {featureName} master", false),
                $"Starting feature {featureName}"
            );
        }
    }
}
