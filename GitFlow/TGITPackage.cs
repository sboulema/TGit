using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using FundaRealEstateBV.TGIT.Helpers;
using FundaRealEstateBV.TGIT.Commands;

namespace FundaRealEstateBV.TGIT
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidTgitPkgString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideOptionPage(typeof(OptionPageGrid), "TGIT", "General", 0, 0, true)]
    public sealed class TgitPackage : Package
    {
        private DTE dte;
        private OptionPageGrid options;       
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

            dte = (DTE)GetService(typeof(DTE));
            options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            fileHelper = new FileHelper(dte);
            processHelper = new ProcessHelper(dte);
            gitHelper = new GitHelper(fileHelper, options.FeatureBranch, options.ReleaseBranch, options.HotfixBranch);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            commandHelper = new CommandHelper(processHelper, fileHelper, mcs);

            new MainMenuCommands(processHelper, commandHelper, gitHelper, fileHelper, dte, options, mcs).AddCommands();

            new ContextMenuCommands(processHelper, commandHelper, gitHelper, fileHelper, dte, options).AddCommands();

            new GitFlowCommands(processHelper, commandHelper, gitHelper, fileHelper, dte, options).AddCommands();

            OleMenuCommand tgitMenu = commandHelper.CreateCommand(null, PkgCmdIDList.TGitMenu);
            OleMenuCommand tgitContextMenu = commandHelper.CreateCommand(null, PkgCmdIDList.TGitContextMenu);
            switch (dte.Version)
            {
                case "11.0":
                case "12.0":
                    tgitMenu.Text = "TGIT";
                    tgitContextMenu.Text = "TGIT";
                    break;
                default:
                    tgitMenu.Text = "Tgit";
                    tgitContextMenu.Text = "Tgit";
                    break;
            }       
            mcs.AddCommand(tgitMenu);
            mcs.AddCommand(tgitContextMenu);
        }
    }
}
