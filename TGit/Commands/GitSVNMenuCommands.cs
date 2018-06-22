using SamirBoulema.TGit.Helpers;
using System;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace SamirBoulema.TGit.Commands
{
    public class GitSVNMenuCommands
    {
        private readonly OptionPageGrid _options;
        private readonly OleMenuCommandService _mcs;
        private readonly DTE _dte;
        private readonly EnvHelper _envHelper;

        public GitSVNMenuCommands(OleMenuCommandService mcs, DTE dte, 
            OptionPageGrid options, EnvHelper envHelper)
        {
            _mcs = mcs;
            _options = options;
            _dte = dte;
            _envHelper = envHelper;
        }

        public void AddCommands()
        {     
            CommandHelper.AddCommand(_mcs, SvnDCommitCommand, PkgCmdIDList.SvnDCommit);
            CommandHelper.AddCommand(_mcs, SvnFetchCommand, PkgCmdIDList.SvnFetch);
            CommandHelper.AddCommand(_mcs, SvnRebaseCommand, PkgCmdIDList.SvnRebase);
        }

        private void SvnDCommitCommand(object sender, EventArgs e)
        {
            FileHelper.SaveAllFiles(_dte);
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:svndcommit /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_options.CloseOnEnd}");
        }

        private void SvnFetchCommand(object sender, EventArgs e)
        {
            FileHelper.SaveAllFiles(_dte);
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:svnrebase /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_options.CloseOnEnd}");
        }

        private void SvnRebaseCommand(object sender, EventArgs e)
        {
            FileHelper.SaveAllFiles(_dte);
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:svnfetch /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_options.CloseOnEnd}");
        }
    }
}
