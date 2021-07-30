using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.commit)]
    internal sealed class CommitCommand : BaseCommand<CommitCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var options = await General.GetLiveInstanceAsync();
            var commitMessage = await GitHelper.GetCommitMessage(options.CommitMessage);
            var bugId = await GitHelper.GetCommitMessage(options.BugId);
            var gitConfig = await GitHelper.GetGitConfig();

            var args = $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)}";

            await ProcessHelper.RunTortoiseGitCommand("commit", args);
        }
    }
}
