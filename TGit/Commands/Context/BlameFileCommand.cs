using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.blameContext)]
    internal sealed class BlameFileCommand : BaseCommand<BlameFileCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            await ProcessHelper.RunTortoiseGitFileCommand(Package, "blame", $"/line:{await FileHelper.GetActiveDocumentCurrentLine()}");
        }
    }
}
