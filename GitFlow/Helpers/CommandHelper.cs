using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGIT.Helpers
{
    public class CommandHelper
    {
        private ProcessHelper processHelper;
        private FileHelper fileHelper;
        private OleMenuCommandService mcs;

        public CommandHelper(ProcessHelper processHelper, FileHelper fileHelper, OleMenuCommandService mcs)
        {
            this.processHelper = processHelper;
            this.fileHelper = fileHelper;
            this.mcs = mcs;
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

        private void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand applyStashCommand = (OleMenuCommand)sender;
            applyStashCommand.Enabled = false;

            if (!string.IsNullOrEmpty(fileHelper.GetSolutionDir()))
            {
                applyStashCommand.Enabled = true;
            }
        }
    }
}
