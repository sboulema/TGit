using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit
{
    [Guid(PackageGuids.guidTGitPkgString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "TGit", "General", 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.GeneralOptions), "TGit", "General", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1, IconMappingFilename = "IconMappings.csv")] 
    public sealed class TGitPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            await this.RegisterCommandsAsync();

            var menuCommandService = (IMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService));

            CommandHelper.AddMenuCommand(menuCommandService, PackageIds.TGitGitFlowMenu, CommandHelper.SolutionVisibility_BeforeQueryStatus);

            CommandHelper.AddMenuCommand(menuCommandService, PackageIds.TGitGitHubFlowMenu, CommandHelper.GitHubFlow_BeforeQueryStatus);

            CommandHelper.AddMenuCommand(menuCommandService, PackageIds.TGitSVNMenu, CommandHelper.GitSvn_BeforeQueryStatus);
        } 
    }
}
