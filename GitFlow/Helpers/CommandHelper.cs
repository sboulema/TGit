using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGIT.Helpers
{
    public class CommandHelper
    {
        private readonly ProcessHelper processHelper;
        private readonly FileHelper fileHelper;
        private readonly GitHelper gitHelper;
        private readonly OleMenuCommandService mcs;
        private readonly OptionPageGrid options;

        public CommandHelper(ProcessHelper processHelper, FileHelper fileHelper, GitHelper gitHelper, OleMenuCommandService mcs, OptionPageGrid options)
        {
            this.processHelper = processHelper;
            this.fileHelper = fileHelper;
            this.gitHelper = gitHelper;
            this.mcs = mcs;
            this.options = options;
        }

        public void AddCommand(EventHandler handler, uint commandId)
        {
            mcs.AddCommand(CreateCommand(handler, commandId));
        }

        public OleMenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            CommandID menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            OleMenuCommand menuItem = new OleMenuCommand(handler, menuCommandId);
            menuItem.BeforeQueryStatus += Solution_BeforeQueryStatus;
            return menuItem;
        }

        public void ApplyStash_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = processHelper.StartProcessGit("stash list");
        }

        private void Diff_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = processHelper.StartProcessGit("diff");
        }

        public void Feature_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = gitHelper.GetCurrentBranchName(false).StartsWith(options.FeatureBranch);
        }

        public void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = gitHelper.GetCurrentBranchName(false).StartsWith(options.HotfixBranch);
        }

        public void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = gitHelper.GetCurrentBranchName(false).StartsWith(options.ReleaseBranch);
        }

        private void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = (OleMenuCommand)sender;
            command.Enabled = false;

            if (!string.IsNullOrEmpty(fileHelper.GetSolutionDir()))
            {
                command.Enabled = true;
            }
        }
    }
}
