﻿using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.fetch)]
    internal sealed class FetchCommand : BaseCommand<FetchCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveAll.ExecuteAsync();

            await ProcessHelper.RunTortoiseGitCommand("fetch");
        }
    }
}
