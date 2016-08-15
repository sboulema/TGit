using EnvDTE;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace SamirBoulema.TGit.Helpers
{
    public class ProcessHelper
    {
        private readonly FileHelper _fileHelper;
        private string _solutionDir;
        private readonly string _tortoiseGitProc, _git;
        private readonly DTE _dte;
        private OutputBox _outputBox;
        private readonly Stopwatch _stopwatch;

        public ProcessHelper(DTE dte)
        {
            _dte = dte;
            _fileHelper = new FileHelper(dte);
            _tortoiseGitProc = _fileHelper.GetTortoiseGitProc();
            _git = _fileHelper.GetMSysGit();
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Execute a Git command and return true if output is non-empty
        /// </summary>
        /// <param name="commands">Git command to be executed</param>
        /// <param name="showAlert">Show an alert dialog when error output is non-empty</param>
        /// <returns>True if output is non-empty</returns>
        public bool StartProcessGit(string commands, bool showAlert = true)
        {
            _solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return false;

            var output = string.Empty;
            var error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{_solutionDir}\" && \"{_git}\" {commands}",
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
            if (!string.IsNullOrEmpty(error) && showAlert)
            {
                MessageBox.Show(error, "TGit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        public void StartTortoiseGitProc(string args)
        {
            try
            {
                Process.Start(_tortoiseGitProc, args);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, $"{_tortoiseGitProc} not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Execute a Git command and return the output
        /// </summary>
        /// <param name="commands">Git command to be executed</param>
        /// <returns>Git output</returns>
        public string StartProcessGitResult(string commands)
        {
            _solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return string.Empty;

            var output = string.Empty;
            var error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{_solutionDir}\" && \"{_git}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output += proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }

            return string.IsNullOrEmpty(output) ? error : output;
        }

        public string GitResult(string workingDir, string commands)
        {
            string output = string.Empty;
            string error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{workingDir}\" && \"{_git}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output += proc.StandardOutput.ReadLine() + ",";
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }

            return string.IsNullOrEmpty(output) ? error : output.TrimEnd(',');
        }

        public void Start(string application)
        {
            Process.Start(application);
        }

        public void Start(string application, string arguments)
        {
            Process.Start(application, arguments);
        }

        public void StartProcessGui(string application, string args, string title)
        {
            var dialogResult = DialogResult.OK;
            if (!StartProcessGit("config user.name") || !StartProcessGit("config user.email"))
            {
                dialogResult = new Credentials(_dte).ShowDialog();
            }

            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo =
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            FileName = application,
                            Arguments = args,
                            WorkingDirectory = _solutionDir
                        },
                        EnableRaisingEvents = true
                    };
                    process.Exited += process_Exited;
                    process.OutputDataReceived += OutputDataHandler;
                    process.ErrorDataReceived += OutputDataHandler;

                    _outputBox = new OutputBox(_dte);

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
                    MessageBox.Show(e.Message, $"{application} not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data)) return;
            var process = sendingProcess as Process;
            if (process == null) return;

            var text = outLine.Data + Environment.NewLine;

            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.AppendText(text, text.StartsWith(">"))));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.Select(_outputBox.richTextBox.TextLength - text.Length + 1, 0)));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.ScrollToCaret()));
        }

        private void process_Exited(object sender, EventArgs e)
        {
            var process = sender as Process;
            if (process == null) return;

            _stopwatch.Stop();

            var exitCodeText = process.ExitCode == 0 ? "Succes" : "Error";
            var summaryText = $"{Environment.NewLine}{exitCodeText} ({_stopwatch.ElapsedMilliseconds} ms @ {process.StartTime})";

            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.AppendText(
                summaryText, 
                process.ExitCode == 0 ? Color.Blue : Color.Red,
                true)));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.Select(_outputBox.richTextBox.TextLength - summaryText.Length + Environment.NewLine.Length, 0)));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.ScrollToCaret()));
            _outputBox.BeginInvoke((Action) (() => _outputBox.okButton.Enabled = true));
        }
    }
}
