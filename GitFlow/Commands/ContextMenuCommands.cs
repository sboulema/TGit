using EnvDTE;
using FundaRealEstateBV.TGIT.Helpers;
using System;

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
            processHelper.StartTortoiseGitProc(string.Format("/command:log /path:\"{0}\" /closeonend:0", currentFilePath));
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
            processHelper.StartTortoiseGitProc(string.Format("/command:repobrowser /path:\"{0}\"", currentFilePath));
        }
        private void BlameContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            int currentLineIndex = ((TextDocument)dte.ActiveDocument.Object(string.Empty)).Selection.CurrentLine;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:blame /path:\"{0}\" /line:{1}", currentFilePath, currentLineIndex));
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:merge /path:\"{0}\"", currentFilePath));
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:pull /path:\"{0}\"", currentFilePath));
        }
        private void FetchContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:fetch /path:\"{0}\"", currentFilePath));
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:commit /path:\"{0}\" /logmsg:\"{1}\" /closeonend:0", currentFilePath, 
                gitHelper.GetCommitMessage(options.CommitMessage, dte)));
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:revert /path:\"{0}\"", currentFilePath));
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();
            processHelper.StartTortoiseGitProc(string.Format("/command:diff /path:\"{0}\"", currentFilePath));
        }
        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            string currentFilePath = dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(currentFilePath)) return;
            dte.ActiveDocument.Save();

            var revisions = processHelper.StartProcessResult(fileHelper.GetMSysGit(), string.Format("log -2 --pretty=format:%h {0}", currentFilePath));
            processHelper.StartTortoiseGitProc(string.Format("/command:diff /path:\"{0}\" /startrev:{1} /endrev:{2}", 
                currentFilePath, revisions.Split(',')[0], revisions.Split(',')[1]));
        }
    }
}
