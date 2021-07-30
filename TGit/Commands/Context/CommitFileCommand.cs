﻿using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.commitContext)]
    internal sealed class CommitFileCommand : BaseCommand<CommitFileCommand>
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

            await ProcessHelper.RunTortoiseGitFileCommand(Package, "commit", args);
        }
    }
}