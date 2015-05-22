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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using FundaRealEstateBV.TGIT.Helpers;

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
        private DTE _dte;
        private string _solutionDir;
        private string _currentFilePath;
        private int _currentLineIndex;
        private OutputBox _outputBox;
        private OptionPageGrid _options;
        private Stopwatch _stopwatch;
        private FileHelper _fileHelper;

        #region Package Members
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _dte = (DTE)GetService(typeof(DTE));
            _options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            _stopwatch = new Stopwatch();
            _fileHelper = new FileHelper(_dte);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            mcs.AddCommand(CreateCommand(StartFeatureCommand, PkgCmdIDList.StartFeature));
            mcs.AddCommand(CreateCommand(FinishFeatureCommand, PkgCmdIDList.FinishFeature));
            mcs.AddCommand(CreateCommand(StartReleaseCommand, PkgCmdIDList.StartRelease));
            mcs.AddCommand(CreateCommand(FinishReleaseCommand, PkgCmdIDList.FinishRelease));

            mcs.AddCommand(CreateCommand(ShowChangesCommand, PkgCmdIDList.ShowChanges));
            mcs.AddCommand(CreateCommand(PullCommand, PkgCmdIDList.Pull));
            mcs.AddCommand(CreateCommand(FetchCommand, PkgCmdIDList.Fetch));
            mcs.AddCommand(CreateCommand(CommitCommand, PkgCmdIDList.Commit));
            //OleMenuCommand commit = CreateCommand(CommitCommand, PkgCmdIDList.Commit);
            //commit.BeforeQueryStatus += Diff_BeforeQueryStatus;
            //mcs.AddCommand(commit);
            mcs.AddCommand(CreateCommand(PushCommand, PkgCmdIDList.Push));

            mcs.AddCommand(CreateCommand(ShowLogCommand, PkgCmdIDList.ShowLog));
            mcs.AddCommand(CreateCommand(DiskBrowserCommand, PkgCmdIDList.DiskBrowser));
            mcs.AddCommand(CreateCommand(RepoBrowserCommand, PkgCmdIDList.RepoBrowser));

            mcs.AddCommand(CreateCommand(CreateStashCommand, PkgCmdIDList.CreateStash));
            OleMenuCommand applyStash = CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash);
            applyStash.BeforeQueryStatus += ApplyStash_BeforeQueryStatus;
            mcs.AddCommand(applyStash);

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
            mcs.AddCommand(CreateCommand(FetchContextCommand, PkgCmdIDList.FetchContext));
            mcs.AddCommand(CreateCommand(CommitContextCommand, PkgCmdIDList.CommitContext));
            mcs.AddCommand(CreateCommand(RevertContextCommand, PkgCmdIDList.RevertContext));
            mcs.AddCommand(CreateCommand(DiffContextCommand, PkgCmdIDList.DiffContext));
            mcs.AddCommand(CreateCommand(PrefDiffContextCommand, PkgCmdIDList.PrefDiffContext));

            OleMenuCommand tgitMenu = CreateCommand(null, PkgCmdIDList.TGitMenu);
            OleMenuCommand tgitContextMenu = CreateCommand(null, PkgCmdIDList.TGitContextMenu);
            switch (_dte.Version)
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

        #endregion

        #region QueryStatus

        private void ApplyStash_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = StartProcessGit("stash list");
        }

        private void Diff_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = StartProcessGit("diff");
        }

        private static void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand applyStashCommand = (OleMenuCommand)sender;
            applyStashCommand.Enabled = false;

            if (!string.IsNullOrEmpty(GetSolutionDir()))
            {
                applyStashCommand.Enabled = true;
            }
        }

        #endregion

        #region Main menu items

        private void StartFeatureCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
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
                    "echo ^> git checkout {2} && git checkout {2} && " +
                    "echo ^> git pull && git pull && " +
                    "echo ^> git checkout -b {3}/{0} {2} && git checkout -b {3}/{0} {2}",
                    featureName, _solutionDir, _options.DevelopBranch, _options.FeatureBranch),
                string.Format("Starting feature {0}", featureName)
            );
        }

        private void FinishFeatureCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string featureName = GetCurrentBranchName(true);

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
                    "echo ^> git checkout {2} && git checkout {2} && " +
                    "echo ^> git pull && git pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && git merge --no-ff {3}/{0} && " +
                    "echo ^> git branch -d {3}/{0} && git branch -d {3}/{0} && " +
                    "echo ^> git push origin --delete {3}/{0} && git push origin --delete {3}/{0} && " +
                    "echo ^> git push origin {2} && git push origin {2}", 
                    featureName, _solutionDir, _options.DevelopBranch, _options.FeatureBranch),
                string.Format("Finishing feature {0}", featureName));
        }

        private void StartReleaseCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string releaseVersion = Interaction.InputBox("Release Version:", "Start New Release");
            if (string.IsNullOrEmpty(releaseVersion)) return;

            /* 1. Switch to the develop branch
             * 2. Pull latest changes on develop
             * 3. Create and switch to a new release branch
             */
            StartProcessGui(
                "cmd.exe",
                string.Format("/c cd {1} && " +
                    "echo ^> git checkout {2} && git checkout {2} && " +
                    "echo ^> git pull && git pull && " +
                    "echo ^> git checkout -b {3}/{0} {2} && git checkout -b {3}/{0} {2}", 
                    releaseVersion, _solutionDir, _options.DevelopBranch, _options.ReleaseBranch),
                string.Format("Starting release {0}", releaseVersion)
            );
        }

        private void FinishReleaseCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            string releaseName = GetCurrentBranchName(true);

            /* 1. Switch to the master branch
             * 2. Pull latest changes on master
             * 3. Merge the release branch to master
             * 4. Tag the release
             * 5. Switch to the develop branch
             * 6. Pull latest changes on develop
             * 7. Merge the release branch to develop
             * 8. Delete the local release branch
             * 9. Delete the remote release branch
             * 10. Push all changes to develop
             * 11. Push all changes to master
             */
            StartProcessGui(
                "cmd.exe",
                string.Format("/c cd {1} && " +
                    "echo ^> git checkout {4} && git checkout {4} && " +
                    "echo ^> git pull && git pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && git merge --no-ff {3}/{0} && " +
                    "echo ^> git tag {0} && git tag {0} && " +
                    "echo ^> git checkout {2} && git checkout {2} && " +
                    "echo ^> git pull && git pull && " +
                    "echo ^> git merge --no-ff {3}/{0} && git merge --no-ff {3}/{0} && " +
                    "echo ^> git branch -d {3}/{0} && git branch -d {3}/{0} && " +
                    "echo ^> git push origin --delete {3}/{0} && git push origin --delete {3}/{0} && " +
                    "echo ^> git push origin {2} && git push origin {2} && " +
                    "echo ^> git push origin {4} && git push origin {4}", 
                    releaseName, _solutionDir, _options.DevelopBranch, _options.ReleaseBranch, _options.MasterBranch),
                string.Format("Finishing release {0}", releaseName));
        }

        private void ShowChangesCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:repostatus /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void PullCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:pull /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:fetch /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:commit /path:\"{0}\" /logmsg:\"{1}\" /closeonend:0", _solutionDir, GetCommitMessage()));
        }
        private void PushCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:push /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:log /path:\"{0}\" /closeonend:0", _solutionDir));
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            Process.Start(_solutionDir);
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:repobrowser /path:\"{0}\"", _solutionDir));
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:stashsave /path:\"{0}\"", _solutionDir));
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:reflog /ref:refs/stash /path:\"{0}\"", _solutionDir));
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:branch /path:\"{0}\"", _solutionDir));
        }
        private void SwitchCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:switch /path:\"{0}\"", _solutionDir));
        }
        private void MergeCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:merge /path:\"{0}\"", _solutionDir));
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            _fileHelper.SaveAllFiles();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:revert /path:\"{0}\"", _solutionDir));
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return;
            StartProcess("TortoiseGitProc.exe", string.Format("/command:cleanup /path:\"{0}\"", _solutionDir));
        }
        #endregion

        #region Context menu items
        private void ShowLogContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
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
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:blame /path:\"{0}\" /line:{1}", _currentFilePath, _currentLineIndex));
        }
        private void MergeContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:merge /path:\"{0}\"", _currentFilePath));
        }
        private void PullContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:pull /path:\"{0}\"", _currentFilePath));
        }
        private void FetchContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:fetch /path:\"{0}\"", _currentFilePath));
        }
        private void CommitContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:commit /path:\"{0}\" /logmsg:\"{1}\" /closeonend:0", _currentFilePath, GetCommitMessage()));
        }
        private void RevertContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:revert /path:\"{0}\"", _currentFilePath));
        }
        private void DiffContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();
            StartProcess("TortoiseGitProc.exe", string.Format("/command:diff /path:\"{0}\"", _currentFilePath));
        }
        private void PrefDiffContextCommand(object sender, EventArgs e)
        {
            _currentFilePath = _dte.ActiveDocument.FullName;
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            _dte.ActiveDocument.Save();

            var revisions = StartProcessResult("git.exe", string.Format("log -2 --pretty=format:%h {0}", _currentFilePath));        
            StartProcess("TortoiseGitProc.exe", string.Format("/command:diff /path:\"{0}\" /startrev:{1} /endrev:{2}", _currentFilePath, revisions.Split(',')[0], revisions.Split(',')[1]));
        }
        #endregion

        private static OleMenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            CommandID menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            OleMenuCommand menuItem = new OleMenuCommand(handler, menuCommandId);
            menuItem.BeforeQueryStatus += Solution_BeforeQueryStatus;
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
            if (!StartProcessGit("config user.name") || !StartProcessGit("config user.email"))
            {
                new Credentials(_dte).ShowDialog();
            }
            else
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

        private bool StartProcessGit(string gitCommands)
        {
            _solutionDir = GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return false;

            string output = string.Empty;
            string error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c {0} && cd {1} && git {2}", Path.GetPathRoot(_solutionDir).TrimEnd('\\'), _solutionDir, gitCommands),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output = proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }
            if (!string.IsNullOrEmpty(output))
            {
                return true;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "TGIT error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        #endregion

        private static string GetSolutionDir()
        {
            var _dte = (DTE)GetGlobalService(typeof(DTE));
            string fileName = _dte.Solution.FullName;
            if (!string.IsNullOrEmpty(fileName))
            {
                var path = Path.GetDirectoryName(fileName);
                return FindGitdir(path);
            }
            return string.Empty;
        }

        private string GetCurrentBranchName(bool trimPrefix)
        {
            string branchName = string.Empty;
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
                branchName = proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }
            if (branchName != null)
            {
                if (branchName.StartsWith(_options.FeatureBranch) && trimPrefix)
                {
                    return branchName.Substring(_options.FeatureBranch.Length + 1);
                }
                else if (branchName.StartsWith(_options.ReleaseBranch) && trimPrefix)
                {
                    return branchName.Substring(_options.ReleaseBranch.Length + 1);
                }
                return branchName;
            }
            if(!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Unable to detect branch name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return branchName;
        }

        private static string FindGitdir(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);
                if (di.GetDirectories().Any(d => d.Name.Equals(".git")))
                {
                    return di.FullName;
                }
                if (di.Parent != null)
                {
                    return FindGitdir(di.Parent.FullName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "TGIT error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }

        private string GetCommitMessage()
        {
            //var projectDir = _dte.Solution.Projects.Item(1).FullName;
            //var projectFileName = _dte.Solution.Projects.Item(1).FileName;

            string commitMessage = _options.CommitMessage;
            commitMessage = commitMessage.Replace("$(BranchName)", GetCurrentBranchName(false));
            commitMessage = commitMessage.Replace("$(FeatureName)", GetCurrentBranchName(true));
            commitMessage = commitMessage.Replace("$(Configuration)", _dte.Solution.SolutionBuild.ActiveConfiguration.Name);
            //commitMessage = commitMessage.Replace("$(Platform)", (string)_dte.Solution.Projects.Item(1).ConfigurationManager.PlatformNames);
            commitMessage = commitMessage.Replace("$(DevEnvDir)", (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VS7\\", _dte.Version, ""));
            //commitMessage = commitMessage.Replace("$(ProjectDir)", Path.GetDirectoryName(_dte.Solution.Projects.Item(1).FullName));
            //commitMessage = commitMessage.Replace("$(ProjectPath)", Path.GetFullPath(_dte.Solution.Projects.Item(1).FullName));
            //commitMessage = commitMessage.Replace("$(ProjectName)", _dte.Solution.Projects.Item(1).FullName);
            //commitMessage = commitMessage.Replace("$(ProjectFileName)", _dte.Solution.Projects.Item(1).FileName);
            //commitMessage = commitMessage.Replace("$(ProjectExt)", Path.GetExtension(_dte.Solution.Projects.Item(1).FileName));
            commitMessage = commitMessage.Replace("$(SolutionDir)", Path.GetDirectoryName(_dte.Solution.FullName));
            commitMessage = commitMessage.Replace("$(SolutionPath)", Path.GetFullPath(_dte.Solution.FullName));
            commitMessage = commitMessage.Replace("$(SolutionName)", _dte.Solution.FullName);
            commitMessage = commitMessage.Replace("$(SolutionFileName)", _dte.Solution.FileName);
            commitMessage = commitMessage.Replace("$(SolutionExt)", Path.GetExtension(_dte.Solution.FileName));
            commitMessage = commitMessage.Replace("$(VSInstallDir)", (string)Registry.GetValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}", _dte.Version), "InstallDir", ""));
            commitMessage = commitMessage.Replace("$(FxCopDir)", (string)Registry.GetValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}\\Edev", _dte.Version), "FxCopDir", ""));
            return commitMessage;
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage
    {
        private string _developBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Develop branch prefix")]
        [Description("Prefix for your Gitflow develop branch")]
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
        [DisplayName(@"Feature branches prefix")]
        [Description("Prefix for your Gitflow feature branches")]
        public string FeatureBranch {
            get
            {
                return string.IsNullOrEmpty(_featureBranch) ? "feature" : _featureBranch;
            }
            set { _featureBranch = value; }
        }

        private string _releaseBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Release branches prefix")]
        [Description("Prefix for your Gitflow release branches")]
        public string ReleaseBranch
        {
            get
            {
                return string.IsNullOrEmpty(_releaseBranch) ? "release" : _releaseBranch;
            }
            set { _releaseBranch = value; }
        }

        private string _masterBranch { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Master branches prefix")]
        [Description("Prefix for your Gitflow master branch")]
        public string MasterBranch
        {
            get
            {
                return string.IsNullOrEmpty(_releaseBranch) ? "master" : _masterBranch;
            }
            set { _masterBranch = value; }
        }

        private string _commitMessage { get; set; }
        [Category("TGIT")]
        [DisplayName(@"Default commit message")]
        [Description("$(BranchName), $(FeatureName), https://msdn.microsoft.com/en-us/library/c02as0cs.aspx")]
        public string CommitMessage
        {
            get
            {
                return string.IsNullOrEmpty(_commitMessage) ? "$(FeatureName)" : _commitMessage;
            }
            set { _commitMessage = value; }
        }
    }
}
