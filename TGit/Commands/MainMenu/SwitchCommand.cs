﻿using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.Switch)]
    internal sealed class SwitchCommand : BaseCommand<SwitchCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            await ProcessHelper.RunTortoiseGitCommand(Package, "switch");
        }
    }
}
