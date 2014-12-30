using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;

namespace FundaRealEstateBV.TGIT
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidTgitPkgString)]
    [ProvideOptionPage(typeof(OptionPageGrid), "TGIT", "General", 0, 0, true)]
    public sealed class TgitPackage : Package
    {
        private DTE _dte;
        private string _solutionDir;
        private string _currentFilePath;
        private int _currentLineIndex;
        private OutputBox _outputBox;
        private OptionPageGrid _options;
        private Stopwatch _stopwatch;

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
            _options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            _stopwatch = new Stopwatch();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            mcs.AddCommand(CreateCommand(StartFeatureCommand, PkgCmdIDList.StartFeature));
            mcs.AddCommand(CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature));

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

            mcs.AddCommand(CreateCommand(ShowLogContextCommand, PkgCmdIDList.ShowLogContext));
            mcs.AddCommand(CreateCommand(DiskBrowserContextCommand, PkgCmdIDList.DiskBrowserContext));
            mcs.AddCommand(CreateCommand(RepoBrowserContextCommand, PkgCmdIDList.RepoBrowserContext));

            mcs.AddCommand(CreateCommand(BlameContextCommand, PkgCmdIDList.BlameContext));

            mcs.AddCommand(CreateCommand(MergeContextCommand, PkgCmdIDList.MergeContext));

            mcs.AddCommand(CreateCommand(PullContextCommand, PkgCmdIDList.PullContext));
            mcs.AddCommand(CreateCommand(CommitContextCommand, PkgCmdIDList.CommitContext));
            mcs.AddCommand(CreateCommand(RevertContextCommand, PkgCmdIDList.RevertContext));
            mcs.AddCommand(CreateCommand(DiffContextCommand, PkgCmdIDList.DiffContext));
            mcs.AddCommand(CreateCommand(PrefDiffContextCommand, PkgCmdIDList.PrefDiffContext));
        }
        #endregion

        #region Main menu items

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = Interaction.InputBox("Feature Name:", "Start New Feature");
            if (string.IsNullOrEmpty(featureName)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new branch
             */
            StartProcessGui(
                "cmd.exe", 
                string.Format("/c cd {1} && " +
                    "git checkout {2} && " +
                    "git pull && " +
                    "git checkout -b {3}/{0} {2}", featureName, _solutionDir, _options.DevelopBranch, _options.FeatureBranch),
                string.Format("Starting feature {0}", featureName)
            );
        }
        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = GetCurrentFeatureName();

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Merge the feature branch to develop
             * 4. Delete the local feature branch
             * 5. Delete the remote feature branch
             * 6. Push all changes to develop
             */
            StartProcessGui(
                "cmd.exe",
                string.Format("/c cd {1} && " +
                    "git checkout {2} && " +
                    "git pull && " +
                    "git merge --no-ff {3}/{0} && " +
                    "git branch -d {3}/{0} && " +
                    "git push origin --delete {3}/{0} && " +
                    "git push origin {2}", featureName, _solutionDir, _options.DevelopBranch, _options.FeatureBranch),
                string.Format("Finishing feature {0}", featureName));
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

        #region Context menu items
        private void ShowLogContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:log /path:\"{0}\" /closeonend:0", _currentFilePath));
        }
        private void DiskBrowserContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            Process.Start(_currentFilePath);
        }
        private void RepoBrowserContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:repobrowser /path:\"{0}\"", _currentFilePath));
        }
        private void BlameContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            _currentLineIndex = ((TextDocument)_dte.ActiveDocument.Object(string.Empty)).Selection.CurrentLine;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:blame /path:\"{0}\" /line:{1}", _currentFilePath, _currentLineIndex));
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:merge /path:\"{0}\"", _currentFilePath));
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:pull /path:\"{0}\"", _currentFilePath));
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:commit /path:\"{0}\"", _currentFilePath));
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:revert /path:\"{0}\"", _currentFilePath));
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:diff /path:\"{0}\"", _currentFilePath));
        }
        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;

            var revisions = StartProcessResult("git.exe", string.Format("log -2 --pretty=format:%h {0}", _currentFilePath));        
            StartProcess("TortoiseGitProc.exe", string.Format("/command:diff /path:\"{0}\" /startrev:{1} /endrev:{2}", _currentFilePath, revisions.Split(',')[0], revisions.Split(',')[1]));
        }
        #endregion

        private static MenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            CommandID menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            MenuCommand menuItem = new MenuCommand(handler, menuCommandId);
            return menuItem;
        }

        #region Process management
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

        private string StartProcessResult(string application, string args)
        {
            string results = string.Empty;
            try
            {
                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = application;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = _solutionDir;

                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    results += process.StandardOutput.ReadLine() + ",";
                }             
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, string.Format("{0} not found", application), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return results;
        }

        private void StartProcessGui(string application, string args, string title)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.Exited += process_Exited;
                process.OutputDataReceived += OutputDataHandler;
                process.ErrorDataReceived += OutputDataHandler;
                process.StartInfo.FileName = application;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = _solutionDir;

                _outputBox = new OutputBox();

                _stopwatch.Reset();
                _stopwatch.Start();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                _outputBox.Text = title;
                _outputBox.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, string.Format("{0} not found", application), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data)) return;
            var process = sendingProcess as Process;
            if (process == null) return;

            _outputBox.BeginInvoke((Action)(() => _outputBox.textBox.AppendText(outLine.Data + "\n")));
            _outputBox.BeginInvoke((Action)(() => _outputBox.textBox.Select(0,0)));
        }

        private void process_Exited(object sender, EventArgs e)
        {
            var process = sender as Process;
            if (process == null) return;

            _stopwatch.Stop();
            var exitCodeText = process.ExitCode == 0 ? "Succes" : "Error";

            _outputBox.BeginInvoke((Action)(() => _outputBox.textBox.AppendText(string.Format("{0}{1} ({2} ms @ {3})", Environment.NewLine, exitCodeText, 
                _stopwatch.ElapsedMilliseconds, process.StartTime))));
            _outputBox.BeginInvoke((Action)(() => _outputBox.okButton.Enabled = true));
        }
        #endregion

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
            string error = string.Empty;
            string drive = Path.GetPathRoot(_solutionDir).TrimEnd('\\');
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c cd {0} && {1} && git symbolic-ref -q --short HEAD", _solutionDir, drive),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                featureName = proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }
            if (featureName != null && featureName.StartsWith(_options.FeatureBranch))
            {
                return featureName.Substring(_options.FeatureBranch.Length + 1);
            }
            if(!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Unable to detect feature name", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage
    {
        private string _developBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Develop branch")]
        [Description("Name of your GIT develop branch")]
        public string DevelopBranch
        {
            get
            {
                return string.IsNullOrEmpty(_developBranch) ? "develop" : _developBranch;
            }
            set { _developBranch = value; }
        }

        private string _featureBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Feature branch")]
        [Description("Name of your GIT feature branch")]
        public string FeatureBranch {
            get
            {
                return string.IsNullOrEmpty(_featureBranch) ? "feature" : _featureBranch;
            }
            set { _featureBranch = value; }
        }
    }
}
