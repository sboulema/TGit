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
        private bool isAsyncLoadSupported;
        private OleMenuCommandService _mcs;
        private OptionPageGrid _options;
        private EnvHelper _envHelper;

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
            _envHelper = new EnvHelper(_dte);
            CommandHelper.EnvHelper = _envHelper;

            _options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));

            // Add our command handlers for menu (commands must exist in the .vsct file)
            _mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        }

        private void MainThreadInitialization()
        {
            if (null == _mcs) return;

            // Add all commands
            new MainMenuCommands(_mcs, _dte, _options, _envHelper).AddCommands();
            new ContextMenuCommands(_mcs, _dte, _options, _envHelper).AddCommands();
            new GitFlowMenuCommands(_mcs, _dte, _options, _envHelper).AddCommands();
            new GitSVNMenuCommands(_mcs, _dte, _options, _envHelper).AddCommands();

            // Add all menus
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.TGitMenu, null, MenuText(_dte.Version));
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.TGitContextMenu, null, MenuText(_dte.Version));
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.Toolbar);
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.TGitGitFlowMenu, CommandHelper.SolutionVisibility_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.TGitGitHubFlowMenu, CommandHelper.GitHubFlow_BeforeQueryStatus);
            CommandHelper.AddCommand(_mcs, null, PkgCmdIDList.TGitSvnMenu, CommandHelper.GitSvn_BeforeQueryStatus);
        }

        private string MenuText(string version)
        {
            switch (_dte.Version)
            {
                case "11.0":
                case "12.0":
                    return "TGIT";
                default:
                    return "TGit";
            }
        }
    }
}
