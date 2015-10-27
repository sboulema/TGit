using EnvDTE;
using FundaRealEstateBV.TGIT.Helpers;
using System;
using System.Windows.Forms;

namespace FundaRealEstateBV.TGIT.Commands
{
    public class ContextMenuCommands
    {
        private ProcessHelper processHelper;
        private CommandHelper commandHelper;
        private FileHelper fileHelper;
        private GitHelper gitHelper;
        private DTE dte;
        private OptionPageGrid options;

        public ContextMenuCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, FileHelper fileHelper, 
            DTE dte, OptionPageGrid options)
        {
            this.processHelper = processHelper;
            this.commandHelper = commandHelper;
            this.gitHelper = gitHelper;
            this.fileHelper = fileHelper;
            this.dte = dte;
            this.options = options;
        }

        public void AddCommands()
        {
            commandHelper.AddCommand(ShowLogContextCommand, PkgCmdIDList.ShowLogContext);
            commandHelper.AddCommand(DiskBrowserContextCommand, PkgCmdIDList.DiskBrowserContext);
            commandHelper.AddCommand(RepoBrowserContextCommand, PkgCmdIDList.RepoBrowserContext);

            commandHelper.AddCommand(BlameContextCommand, PkgCmdIDList.BlameContext);

            commandHelper.AddCommand(MergeContextCommand, PkgCmdIDList.MergeContext);

            commandHelper.AddCommand(PullContextCommand, PkgCmdIDList.PullContext);
            commandHelper.AddCommand(FetchContextCommand, PkgCmdIDList.FetchContext);
            commandHelper.AddCommand(CommitContextCommand, PkgCmdIDList.CommitContext);
            commandHelper.AddCommand(RevertContextCommand, PkgCmdIDList.RevertContext);
            commandHelper.AddCommand(DiffContextCommand, PkgCmdIDList.DiffContext);
            commandHelper.AddCommand(PrefDiffContextCommand, PkgCmdIDList.PrefDiffContext);
        }

        private void ShowLogContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:log /path:\"{currentFilePath}\" /closeonend:0");
        }
        private void DiskBrowserContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            processHelper.Start(currentFilePath);
        }
        private void RepoBrowserContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            processHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{currentFilePath}\"");
        }
        private void BlameContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            int currentLineIndex = ((TextDocument)dte.ActiveDocument.Object(string.Empty)).Selection.CurrentLine;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:blame /path:\"{currentFilePath}\" /line:{currentLineIndex}");
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:merge /path:\"{currentFilePath}\"");
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:pull /path:\"{currentFilePath}\"");
        }
        private void FetchContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:fetch /path:\"{currentFilePath}\"");
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:commit /path:\"{currentFilePath}\" /logmsg:\"{gitHelper.GetCommitMessage(options.CommitMessage, dte)}\" /closeonend:0");
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:revert /path:\"{currentFilePath}\"");
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc($"/command:diff /path:\"{currentFilePath}\"");
        }

        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();

            var revisions = processHelper.StartProcessGitResult($"log -2 --pretty=format:%h {currentFilePath}");
            if (!revisions.Contains(","))
            {
                MessageBox.Show("Could not determine the last committed revision!", "TGIT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                processHelper.StartTortoiseGitProc($"/command:diff /path:\"{currentFilePath}\" /startrev:{revisions.Split(',')[0]} /endrev:{revisions.Split(',')[1]}");
            }
        }
    }
}
