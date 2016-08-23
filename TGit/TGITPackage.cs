using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SamirBoulema.TGit.Helpers;
using SamirBoulema.TGit.Commands;

namespace SamirBoulema.TGit
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1, IconMappingFilename = "IconMappings.csv")]
    [Guid(GuidList.GuidTgitPkgString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideOptionPage(typeof(OptionPageGrid), "TGit", "General", 0, 0, true)]
    // ReSharper disable once InconsistentNaming
    public sealed class TGitPackage : Package
    {
        private DTE _dte;
        private CommandHelper _commandHelper;
        private SolutionEvents _events;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _dte = (DTE)GetService(typeof(DTE));        
            var options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));

            _events = _dte.Events.SolutionEvents;
            _events.Opened += SolutionEvents_Opened;
            _events.AfterClosing += SolutionEvents_AfterClosing;

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            _commandHelper = new CommandHelper(mcs);

            new MainMenuCommands(_commandHelper, _dte, options).AddCommands();

            new ContextMenuCommands(_commandHelper, _dte, options).AddCommands();

            new GitFlowMenuCommands(_commandHelper, options).AddCommands();

            // Add all menus
            var tgitMenu = _commandHelper.CreateCommand(PkgCmdIDList.TGitMenu);
            var tgitContextMenu = _commandHelper.CreateCommand(PkgCmdIDList.TGitContextMenu);
            switch (_dte.Version)
            {
                case "11.0":
                case "12.0":
                    tgitMenu.Text = "TGIT";
                    tgitContextMenu.Text = "TGIT";
                    break;
                default:
                    tgitMenu.Text = "TGit";
                    tgitContextMenu.Text = "TGit";
                    break;
            }       
            mcs.AddCommand(tgitMenu);
            mcs.AddCommand(tgitContextMenu);

            var tgitGitFlowMenu = _commandHelper.CreateCommand(PkgCmdIDList.TGitGitFlowMenu);
            tgitGitFlowMenu.BeforeQueryStatus += _commandHelper.SolutionVisibility_BeforeQueryStatus;
            mcs.AddCommand(tgitGitFlowMenu);

            var tgitGitHubFlowMenu = _commandHelper.CreateCommand(PkgCmdIDList.TGitGitHubFlowMenu);
            tgitGitHubFlowMenu.BeforeQueryStatus += _commandHelper.GitHubFlow_BeforeQueryStatus;
            mcs.AddCommand(tgitGitHubFlowMenu);
        }

        private static void SolutionEvents_AfterClosing()
        {
            EnvHelper.Clear();
        }

        public void SolutionEvents_Opened()
        {
            EnvHelper.GetTortoiseGitProc();
            EnvHelper.GetGit();
            EnvHelper.GetSolutionDir(_dte);
            EnvHelper.GetFlowOptions();
            EnvHelper.GetBranchName();
            EnvHelper.GetStash();
        }     
    }
}
