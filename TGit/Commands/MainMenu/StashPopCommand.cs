using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.StashPop)]
    internal sealed class StashPopCommand : BaseCommand<StashPopCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            await ProcessHelper.RunTortoiseGitCommand(Package, "stashpop");
        }

        protected override void BeforeQueryStatus(EventArgs e)
        {
            Command.Enabled = GitHelper.HasStash().Result;
        }
    }
}
