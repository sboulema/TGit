using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.init)]
    internal sealed class GitFlowInitCommand : BaseCommand<GitFlowInitCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var flowDialog = new FlowDialog();

            if (flowDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var versionTag = string.IsNullOrEmpty(flowDialog.GitConfig.TagPrefix) ? "\"\"" : flowDialog.GitConfig.TagPrefix;

            /* 1. Add GitFlow config options
             * 2. Checkout develop branch (create if it doesn't exist, reset if it does)
             * 3. Push develop branch
             */
            var process = await ProcessHelper.StartProcessGui(
                "cmd.exe",
                $"/c cd \"{await FileHelper.GetSolutionDir()}\" && " +
                await GitHelper.GetSshSetup() +
                GitHelper.FormatCliCommand($"config --add gitflow.branch.master {flowDialog.GitConfig.MasterBranch}") +
                GitHelper.FormatCliCommand($"config --add gitflow.branch.develop {flowDialog.GitConfig.DevelopBranch}") +
                GitHelper.FormatCliCommand($"config --add gitflow.prefix.feature {flowDialog.GitConfig.FeaturePrefix}") +
                GitHelper.FormatCliCommand($"config --add gitflow.prefix.release {flowDialog.GitConfig.ReleasePrefix}") +
                GitHelper.FormatCliCommand($"config --add gitflow.prefix.hotfix {flowDialog.GitConfig.HotfixPrefix}") +
                GitHelper.FormatCliCommand($"config --add gitflow.prefix.versiontag {versionTag}") +
                (await GitHelper.RemoteBranchExists(flowDialog.GitConfig.DevelopBranch) ?
                    "echo." :
                    GitHelper.FormatCliCommand($"checkout -b {flowDialog.GitConfig.DevelopBranch}", false)),
                "Initializing GitFlow"
                );
            process.WaitForExit();
        }

        protected override async void BeforeQueryStatus(System.EventArgs e)
        {
            Command.Enabled = await FileHelper.HasSolutionDir();
            Command.Text = await GitHelper.IsGitFlow() ? "Config" : "Init";
        }
    }
}
