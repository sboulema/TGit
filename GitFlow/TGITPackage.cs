using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using FundaRealEstateBV.GitFlow;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;

namespace FundaRealEstateBV.TGIT
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidGitFlowPkgString)]
    public sealed class GitFlowPackage : Package
    {
        private DTE _dte;
        private string _solutionDir;

        #region Package Members
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _dte = (DTE)GetService(typeof(DTE));
            _solutionDir = GetSolutionDir(); 

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            mcs.AddCommand(CreateCommand(StartFeatureCommand, PkgCmdIDList.StartFeature));
            mcs.AddCommand(CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature));
            mcs.AddCommand(CreateCommand(PushFeatureCommand, PkgCmdIDList.PushFeature));

            mcs.AddCommand(CreateCommand(ShowChangesCommand, PkgCmdIDList.ShowChanges));
            mcs.AddCommand(CreateCommand(PullCommand, PkgCmdIDList.Pull));
            mcs.AddCommand(CreateCommand(CommitCommand, PkgCmdIDList.Commit));
            mcs.AddCommand(CreateCommand(PushCommand, PkgCmdIDList.Push));

            mcs.AddCommand(CreateCommand(ShowLogCommand, PkgCmdIDList.ShowLog));
            mcs.AddCommand(CreateCommand(DiskBrowserCommand, PkgCmdIDList.DiskBrowser));
            mcs.AddCommand(CreateCommand(RepoBrowserCommand, PkgCmdIDList.RepoBrowser));

            mcs.AddCommand(CreateCommand(CreateStashCommand, PkgCmdIDList.CreateStash));
            mcs.AddCommand(CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash));

            mcs.AddCommand(CreateCommand(BranchCommand, PkgCmdIDList.Branch));
            mcs.AddCommand(CreateCommand(SwitchCommand, PkgCmdIDList.Switch));
            mcs.AddCommand(CreateCommand(MergeCommand, PkgCmdIDList.Merge));

            mcs.AddCommand(CreateCommand(RevertCommand, PkgCmdIDList.Revert));
            mcs.AddCommand(CreateCommand(CleanupCommand, PkgCmdIDList.Cleanup));
        }
        #endregion

        #region Button commands
        private void StartFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            StartProcess("cmd.exe", string.Format("/c cd {0} && git checkout develop && git pull && git flow feature start {1}", _solutionDir, featureName));
        }
        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = GetCurrentFeatureName();
            StartProcess("cmd.exe", string.Format("/c cd {0} && git checkout develop && git pull && git flow feature finish {1}", _solutionDir, featureName));
        }
        private void PushFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = GetCurrentFeatureName();
            StartProcess("cmd.exe", string.Format("/c cd {0} && git flow feature publish {1}", _solutionDir, featureName));
        }
        private void ShowChangesCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:repostatus /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void PullCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _dte.Documents.SaveAll();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:pull /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = GetCurrentFeatureName();
            _dte.Documents.SaveAll();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:commit /path:\"{0}\" /logmsg:\"{1}\" /closeonend:0", _solutionDir, featureName));
        }
        private void PushCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:push /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:log /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            Process.Start(_solutionDir);
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:repobrowser /path:\"{0}\"", _solutionDir));
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:stashsave /path:\"{0}\"", _solutionDir));
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:reflog /ref:refs/stash /path:\"{0}\"", _solutionDir));
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:branch /path:\"{0}\"", _solutionDir));
        }
        private void SwitchCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:switch /path:\"{0}\"", _solutionDir));
        }
        private void MergeCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _dte.Documents.SaveAll();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:merge /path:\"{0}\"", _solutionDir));
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:revert /path:\"{0}\"", _solutionDir));
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:cleanup /path:\"{0}\"", _solutionDir));
        }
        #endregion

        private static MenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            CommandID menuCommandId = new CommandID(GuidList.guidGitFlowCmdSet, (int)commandId);
            MenuCommand menuItem = new MenuCommand(handler, menuCommandId);
            return menuItem;
        }

        private static void StartProcess(string application, string args)
        {
            try
            {
                Process.Start(application, args);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, string.Format("{0} not found", application), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSolutionDir()
        {
            string fileName = _dte.Solution.FullName;
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            var path = Path.GetDirectoryName(fileName);
            return FindGitdir(path);
        }

        private string GetCurrentFeatureName()
        {
            string featureName = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c cd {0} && git symbolic-ref -q --short HEAD", _solutionDir),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                featureName = proc.StandardOutput.ReadLine();
            }
            if (featureName != null && featureName.StartsWith("feature/"))
            {
                return featureName.Substring(8);
            }
            return featureName;
        }

        private static string FindGitdir(string path)
        {
            var di = new DirectoryInfo(path);
            if (di.GetDirectories().Any(d => d.Name.Equals(".git")))
            {
                return di.FullName;
            }
            return di.Parent != null ? FindGitdir(di.Parent.FullName) : string.Empty;
        }
    }
}
