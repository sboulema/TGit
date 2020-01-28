using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace SamirBoulema.TGit.Commands
{
    public class ContextMenuCommands
    {
        private readonly DTE _dte;
        private readonly OptionPageGrid _generalOptions;
        private readonly OleMenuCommandService _mcs;
        private readonly EnvHelper _envHelper;

        public ContextMenuCommands(OleMenuCommandService mcs, DTE dte, 
            OptionPageGrid generalOptions, EnvHelper envHelper)
        {
            _dte = dte;
            _mcs = mcs;
            _generalOptions = generalOptions;
            _envHelper = envHelper;
        }

        public void AddCommands()
        {
            CommandHelper.AddCommand(_mcs, ShowLogContextCommand, PkgCmdIDList.ShowLogContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, DiskBrowserContextCommand, PkgCmdIDList.DiskBrowserContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, RepoBrowserContextCommand, PkgCmdIDList.RepoBrowserContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, RevGraphContextCommand, PkgCmdIDList.RevGraphContext, ActiveDocument_BeforeQueryStatus);

            CommandHelper.AddCommand(_mcs, BlameContextCommand, PkgCmdIDList.BlameContext, ActiveDocument_BeforeQueryStatus);

            CommandHelper.AddCommand(_mcs, MergeContextCommand, PkgCmdIDList.MergeContext, ActiveDocument_BeforeQueryStatus);

            CommandHelper.AddCommand(_mcs, PullContextCommand, PkgCmdIDList.PullContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, FetchContextCommand, PkgCmdIDList.FetchContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, CommitContextCommand, PkgCmdIDList.CommitContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, RevertContextCommand, PkgCmdIDList.RevertContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, AddContextCommand, PkgCmdIDList.AddContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, DiffContextCommand, PkgCmdIDList.DiffContext, ActiveDocument_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, PrefDiffContextCommand, PkgCmdIDList.PrefDiffContext, ActiveDocument_BeforeQueryStatus);
        }

        private void ActiveDocument_BeforeQueryStatus(object sender, EventArgs e) 
            => ((OleMenuCommand)sender).Enabled = _dte.ActiveDocument != null;

        private void ShowLogContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:log /path:\"{currentFilePath}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            ProcessHelper.Start(currentFilePath);
        }
        private void RepoBrowserContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repobrowser /path:\"{currentFilePath}\"");
        }
        private void RevGraphContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:revisiongraph /path:\"{currentFilePath}\"");
        }
        private void BlameContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            var currentLineIndex = ((TextDocument)_dte.ActiveDocument.Object(string.Empty)).Selection.CurrentLine;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:blame /path:\"{currentFilePath}\" /line:{currentLineIndex}");
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:merge /path:\"{currentFilePath}\"");
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:pull /path:\"{currentFilePath}\"");
        }
        private void AddContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:add /path:\"{currentFilePath}\"");
        }
        private void FetchContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:fetch /path:\"{currentFilePath}\"");
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            var commitMessage = GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte, _envHelper);
            var bugId = GitHelper.GetCommitMessage(_generalOptions.BugId, _dte, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);

            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:commit /path:\"{currentFilePath}\" " +
                $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)} " +
                $"/closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:revert /path:\"{currentFilePath}\"");
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:diff /path:\"{currentFilePath}\"");
        }

        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            var currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            if (!_dte.ActiveDocument.Saved)
                _dte.ActiveDocument.Save();

            var revisions = ProcessHelper.GitResult(_envHelper, 
                Path.GetDirectoryName(currentFilePath), $"log -2 --pretty=format:%h {FileHelper.GetExactFileName(currentFilePath)}");
            if (!revisions.Contains(","))
            {
                MessageBox.Show("Could not determine the last committed revision!", "TGit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ProcessHelper.StartTortoiseGitProc(_envHelper, 
                    $"/command:diff /path:\"{FileHelper.GetExactPathName(currentFilePath)}\" /startrev:{revisions.Split(',')[0]} /endrev:{revisions.Split(',')[1]}");
            }
        }
    }
}
