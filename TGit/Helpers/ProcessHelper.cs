using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Helpers
{
    public static class ProcessHelper
    {
        private static OutputBox _outputBox;

        /// <summary>
        /// Execute a Git command and return true if output is non-empty
        /// </summary>
        /// <param name="commands">Git command to be executed</param>
        /// <param name="showAlert">Show an alert dialog when error output is non-empty</param>
        /// <returns>True if output is non-empty</returns>
        public static async Task<bool> StartProcessGit(string commands, bool showAlert = true)
        {
            var solutionDir = await FileHelper.GetSolutionDir();

            if (string.IsNullOrEmpty(solutionDir))
            {
                return false;
            }

            var output = string.Empty;
            var error = string.Empty;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{solutionDir}\" && \"{FileHelper.GetMSysGit()}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                output = await proc.StandardOutput.ReadLineAsync();
            }

            while (!proc.StandardError.EndOfStream)
            {
                error += await proc.StandardError.ReadLineAsync();
            }

            if (!string.IsNullOrEmpty(output))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(error) && showAlert)
            {
                await VS.MessageBox.ShowErrorAsync("TGit error", error);
            }

            return false;
        }

        public static async Task StartTortoiseGitProc(string args)
        {
            var tortoiseGitProc = FileHelper.GetTortoiseGitProc();

            try
            {
                Process.Start(tortoiseGitProc, args);
            }
            catch (Exception e)
            {
                await VS.MessageBox.ShowErrorAsync($"{tortoiseGitProc} not found", e.Message);
            }
        }

        /// <summary>
        /// Execute a Git command and return the output
        /// </summary>
        /// <param name="commands">Git command to be executed</param>
        /// <returns>Git output</returns>
        public static async Task<string> StartProcessGitResult(string commands)
        {
            var solutionDir = await FileHelper.GetSolutionDir();

            if (string.IsNullOrEmpty(solutionDir))
            {
                return string.Empty;
            }

            var output = string.Empty;
            var error = string.Empty;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{solutionDir}\" && \"{FileHelper.GetMSysGit()}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                output += await proc.StandardOutput.ReadLineAsync() + ";";
            }

            while (!proc.StandardError.EndOfStream)
            {
                error += await proc.StandardError.ReadLineAsync();
            }

            return string.IsNullOrEmpty(output) ? error : output.TrimEnd(';');
        }

        public static async Task<string> GitResult(string workingDir, string commands)
        {
            var output = string.Empty;
            var error = string.Empty;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{workingDir}\" && \"{FileHelper.GetMSysGit()}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                output += await proc.StandardOutput.ReadLineAsync() + ",";
            }

            while (!proc.StandardError.EndOfStream)
            {
                error += await proc.StandardError.ReadLineAsync();
            }

            return string.IsNullOrEmpty(output) ? error : output.TrimEnd(',');
        }

        public static void Start(string application)
        {
            Process.Start(application);
        }

        public static void Start(string application, string arguments)
        {
            Process.Start(application, arguments);
        }

        public static async Task<Process> StartProcessGui(string application, string args, string title, string branchName = "", 
            OutputBox outputBox = null, OptionPageGrid options = null, string pushCommand = "", AsyncPackage package = null)
        {
            var dialogResult = DialogResult.OK;

            if (!await StartProcessGit("config user.name") ||
                !await StartProcessGit("config user.email"))
            {
                dialogResult = new Credentials().ShowDialog();
            }

            if (dialogResult != DialogResult.OK)
            {
                return null;
            }

            try
            {
                var solutionDir = await FileHelper.GetSolutionDir();

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
                            WorkingDirectory = solutionDir
                        },
                    EnableRaisingEvents = true
                };

                process.Exited += process_Exited;
                process.OutputDataReceived += OutputDataHandler;
                process.ErrorDataReceived += OutputDataHandler;

                if (outputBox == null)
                {
                    _outputBox = new OutputBox(package, branchName, options, pushCommand);
                    _outputBox.Show();
                }
                else
                {
                    _outputBox = outputBox;
                }

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!string.IsNullOrEmpty(title))
                {
                    _outputBox.Text = title;
                }

                _outputBox.Show();

                return process;
            }
            catch (Exception e)
            {
                await VS.MessageBox.ShowErrorAsync($"'{application}' not found", e.Message);
            }

            return null;
        }

        private static void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
            {
                return;
            }

            if (!(sendingProcess is Process process))
            {
                return;
            }

            var text = outLine.Data + Environment.NewLine;

            if (_outputBox.InvokeRequired)
            {
                _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.AppendText(text, text.StartsWith(">"))));
                _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.Select(_outputBox.richTextBox.TextLength - text.Length + 1, 0)));
                _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.ScrollToCaret()));
            }
            else
            {
                _outputBox.richTextBox.AppendText(text, text.StartsWith(">"));
                _outputBox.richTextBox.Select(_outputBox.richTextBox.TextLength - text.Length + 1, 0);
                _outputBox.richTextBox.ScrollToCaret();
            }
        }

        private static void process_Exited(object sender, EventArgs e)
        {
            if (!(sender is Process process))
            {
                return;
            }

            var exitCodeText = process.ExitCode == 0 ? "Success" : "Error";
            var summaryText = $"{Environment.NewLine}{exitCodeText} ({(int)(process.ExitTime - process.StartTime).TotalMilliseconds} ms @ {process.ExitTime})";

            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.AppendText(
                summaryText, 
                process.ExitCode == 0 ? Color.Blue : Color.Red,
                true)));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.Select(_outputBox.richTextBox.TextLength - summaryText.Length + Environment.NewLine.Length, 0)));
            _outputBox.BeginInvoke((Action) (() => _outputBox.richTextBox.ScrollToCaret()));
            _outputBox.BeginInvoke((Action) (() => _outputBox.okButton.Enabled = true));
        }

        public static async Task RunTortoiseGitCommand(AsyncPackage package, string command, string args = "")
        {
            var solutionDir = await FileHelper.GetSolutionDir();

            if (string.IsNullOrEmpty(solutionDir))
            {
                return;
            }

            var options = (OptionPageGrid)package.GetDialogPage(typeof(OptionPageGrid));

            await StartTortoiseGitProc($"/command:{command} /path:\"{solutionDir}\" {args} /closeonend:{options.CloseOnEnd}");
        }

        public static async Task RunTortoiseGitFileCommand(AsyncPackage package, string command, string args = "", string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = await FileHelper.GetActiveDocumentFilePath();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var options = (OptionPageGrid)package.GetDialogPage(typeof(OptionPageGrid));

            await StartTortoiseGitProc($"/command:{command} /path:\"{filePath}\" {args} /closeonend:{options.CloseOnEnd}");
        }

        public static OptionPageGrid GetOptions(AsyncPackage package)
            => (OptionPageGrid)package.GetDialogPage(typeof(OptionPageGrid));
    }
}
