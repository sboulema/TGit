using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;
using System.IO;
using System.Windows.Forms;

namespace SamirBoulema.TGit.Commands
{
    public class ContextMenuCommands
    {
        private readonly ProcessHelper _processHelper;
        private readonly CommandHelper _commandHelper;
        private readonly GitHelper _gitHelper;
        private readonly DTE _dte;
        private readonly OptionPageGrid _generalOptions;

        public ContextMenuCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, 
            DTE dte, OptionPageGrid generalOptions)
        {
            _processHelper = processHelper;
            _commandHelper = commandHelper;
            _gitHelper = gitHelper;
            _dte = dte;
            _generalOptions = generalOptions;
        }

        public void AddCommands()
        {
            _commandHelper.AddCommand(ShowLogContextCommand, PkgCmdIDList.ShowLogContext);
            _commandHelper.AddCommand(DiskBrowserContextCommand, PkgCmdIDList.DiskBrowserContext);
            _commandHelper.AddCommand(RepoBrowserContextCommand, PkgCmdIDList.RepoBrowserContext);

            _commandHelper.AddCommand(BlameContextCommand, PkgCmdIDList.BlameContext);

            _commandHelper.AddCommand(MergeContextCommand, PkgCmdIDList.MergeContext);

            _commandHelper.AddCommand(PullContextCommand, PkgCmdIDList.PullContext);
            _commandHelper.AddCommand(FetchContextCommand, PkgCmdIDList.FetchContext);
            _commandHelper.AddCommand(CommitContextCommand, PkgCmdIDList.CommitContext);
            _commandHelper.AddCommand(RevertContextCommand, PkgCmdIDList.RevertContext);
            _commandHelper.AddCommand(DiffContextCommand, PkgCmdIDList.DiffContext);
            _commandHelper.AddCommand(PrefDiffContextCommand, PkgCmdIDList.PrefDiffContext);
        }

        private void ShowLogContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:log /path:\"{currentFilePath}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _processHelper.Start(currentFilePath);
        }
        private void RepoBrowserContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _processHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{currentFilePath}\"");
        }
        private void BlameContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            int currentLineIndex = ((TextDocument)_dte.ActiveDocument.Object(string.Empty)).Selection.CurrentLine;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:blame /path:\"{currentFilePath}\" /line:{currentLineIndex}");
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:merge /path:\"{currentFilePath}\"");
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:pull /path:\"{currentFilePath}\"");
        }
        private void FetchContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:fetch /path:\"{currentFilePath}\"");
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:commit /path:\"{currentFilePath}\" /logmsg:\"{_gitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:revert /path:\"{currentFilePath}\"");
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();
            _processHelper.StartTortoiseGitProc($"/command:diff /path:\"{currentFilePath}\"");
        }

        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            _dte.ActiveDocument.Save();

            var revisions = _processHelper.GitResult(Path.GetDirectoryName(currentFilePath), $"log -2 --pretty=format:%h {FileHelper.GetExactFileName(currentFilePath)}");
            if (!revisions.Contains(","))
            {
                MessageBox.Show("Could not determine the last committed revision!", "TGit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _processHelper.StartTortoiseGitProc($"/command:diff /path:\"{FileHelper.GetExactPathName(currentFilePath)}\" /startrev:{revisions.Split(',')[0]} /endrev:{revisions.Split(',')[1]}");
            }
        }
    }
}
