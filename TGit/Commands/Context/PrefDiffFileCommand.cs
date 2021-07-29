using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.PrefDiffContext)]
    internal sealed class PrefDiffFileCommand : BaseCommand<PrefDiffFileCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            var filePath = await FileHelper.GetActiveDocumentFilePath();
            var exactFilePath = FileHelper.GetExactFileName(filePath);

            var revisions = await ProcessHelper.GitResult(Path.GetDirectoryName(filePath), $"log -2 --pretty=format:%h {exactFilePath}");

            if (!revisions.Contains(","))
            {
                await VS.MessageBox.ShowErrorAsync("Could not determine the last committed revision!");
                return;
            }

            await ProcessHelper.RunTortoiseGitFileCommand(Package, "diff", $"/startrev:{revisions.Split(',')[0]} /endrev:{revisions.Split(',')[1]}", exactFilePath);
        }
    }
}
