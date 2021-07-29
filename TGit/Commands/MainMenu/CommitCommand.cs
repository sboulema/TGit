using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.Commit)]
    internal sealed class CommitCommand : BaseCommand<CommitCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            var options = (OptionPageGrid)Package.GetDialogPage(typeof(OptionPageGrid));
            var commitMessage = await GitHelper.GetCommitMessage(options.CommitMessage);
            var bugId = await GitHelper.GetCommitMessage(options.BugId);
            var gitConfig = await GitHelper.GetGitConfig();

            var args = $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)}";

            await ProcessHelper.RunTortoiseGitCommand(Package, "commit", args);
        }
    }
}
