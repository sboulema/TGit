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
    public sealed class TGitPackage : Package
    {
        private DTE _dte;     
        private FileHelper fileHelper;
        private ProcessHelper processHelper;
        private CommandHelper commandHelper;
        private GitHelper gitHelper;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _dte = (DTE)GetService(typeof(DTE));        
            var generalOptions = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            fileHelper = new FileHelper(_dte);
            processHelper = new ProcessHelper(_dte);
            gitHelper = new GitHelper(fileHelper, processHelper);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            commandHelper = new CommandHelper(processHelper, fileHelper, gitHelper, mcs);

            new MainMenuCommands(processHelper, commandHelper, gitHelper, fileHelper, _dte, generalOptions, mcs).AddCommands();

            new ContextMenuCommands(processHelper, commandHelper, gitHelper, fileHelper, _dte, generalOptions).AddCommands();

            new GitFlowCommands(processHelper, commandHelper, gitHelper, fileHelper, mcs).AddCommands();

            // Add all menus
            var tgitMenu = commandHelper.CreateCommand(PkgCmdIDList.TGitMenu);
            var tgitContextMenu = commandHelper.CreateCommand(PkgCmdIDList.TGitContextMenu);
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

            var tgitGitFlowMenu = commandHelper.CreateCommand(PkgCmdIDList.TGitGitFlowMenu);
            //tgitGitFlowMenu.BeforeQueryStatus += commandHelper.GitFlow_BeforeQueryStatus;
            mcs.AddCommand(tgitGitFlowMenu);

            var tgitGitHubFlowMenu = commandHelper.CreateCommand(PkgCmdIDList.TGitGitHubFlowMenu);
            tgitGitHubFlowMenu.BeforeQueryStatus += commandHelper.GitHubFlow_BeforeQueryStatus;
            mcs.AddCommand(tgitGitHubFlowMenu);
        }
    }
}
