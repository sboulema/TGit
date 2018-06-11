extern alias vsshell14;

using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SamirBoulema.TGit.Helpers;
using SamirBoulema.TGit.Commands;
using VsShell14 = vsshell14::Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers.AsyncPackageHelpers;
using System;

namespace SamirBoulema.TGit
{
    [AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [VsShell14.ProvideMenuResource("Menus.ctmenu", 1, IconMappingFilename = "IconMappings.csv")]
    //[VsShell14.ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidTgitPkgString)]
    [Helpers.AsyncPackageHelpers.ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(OptionPageGrid), "TGit", "General", 0, 0, true)]
    public sealed class TGitPackage : Package, IAsyncLoadablePackageInitialize
    {
        private DTE _dte;
        private SolutionEvents _events;
        private bool isAsyncLoadSupported;
        private OleMenuCommandService _mcs;
        private OptionPageGrid _options;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            isAsyncLoadSupported = this.IsAsyncPackageSupported();

            // Only perform initialization if async package framework is not supported
            if (!isAsyncLoadSupported)
            {
                BackgroundThreadInitialization();
                MainThreadInitialization();
            }
        }

        /// <summary>
        /// Performs the asynchronous initialization for the package in cases where IDE supports AsyncPackage.
        /// 
        /// This method is always called from background thread initially.
        /// </summary>
        /// <param name="asyncServiceProvider">Async service provider instance to query services asynchronously</param>
        /// <param name="pProfferService">Async service proffer instance</param>
        /// <param name="IAsyncProgressCallback">Progress callback instance</param>
        /// <returns></returns>
        public IVsTask Initialize(Microsoft.VisualStudio.Shell.Interop.IAsyncServiceProvider asyncServiceProvider, 
            IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            if (!isAsyncLoadSupported)
            {
                throw new InvalidOperationException("Async Initialize method should not be called when async load is not supported.");
            }

            return ThreadHelper.JoinableTaskFactory.RunAsync<object>(async () =>
            {
                BackgroundThreadInitialization();

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                MainThreadInitialization();
                return null;
            }).AsVsTask();
        }

        private void BackgroundThreadInitialization()
        {
            _dte = (DTE)GetService(typeof(DTE));
            _options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));

            _events = _dte.Events.SolutionEvents;
            _events.Opened += SolutionEvents_Opened;
            _events.AfterClosing += SolutionEvents_AfterClosing;

            // Add our command handlers for menu (commands must exist in the .vsct file)
            _mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        }

        private void MainThreadInitialization()
        {
            if (null == _mcs) return;

            new MainMenuCommands(_mcs, _dte, _options).AddCommands();

            new ContextMenuCommands(_mcs, _dte, _options).AddCommands();

            new GitFlowMenuCommands(_mcs, _options).AddCommands();

            // Add all menus
            var tgitMenu = CommandHelper.CreateCommand(PkgCmdIDList.TGitMenu);
            var tgitContextMenu = CommandHelper.CreateCommand(PkgCmdIDList.TGitContextMenu);
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
            _mcs.AddCommand(tgitMenu);
            _mcs.AddCommand(tgitContextMenu);

            var tgitGitFlowMenu = CommandHelper.CreateCommand(PkgCmdIDList.TGitGitFlowMenu);
            tgitGitFlowMenu.BeforeQueryStatus += CommandHelper.SolutionVisibility_BeforeQueryStatus;
            _mcs.AddCommand(tgitGitFlowMenu);

            var tgitGitHubFlowMenu = CommandHelper.CreateCommand(PkgCmdIDList.TGitGitHubFlowMenu);
            tgitGitHubFlowMenu.BeforeQueryStatus += CommandHelper.GitHubFlow_BeforeQueryStatus;
            _mcs.AddCommand(tgitGitHubFlowMenu);
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
            EnvHelper.GetGitConfig();
            EnvHelper.GetBranchName();
            EnvHelper.GetStash();
        }     
    }
}
