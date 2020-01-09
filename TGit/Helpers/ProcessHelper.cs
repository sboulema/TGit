using EnvDTE;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

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
        public static bool StartProcessGit(EnvHelper envHelper, string commands, bool showAlert = true)
        {
            if (string.IsNullOrEmpty(envHelper.GetSolutionDir())) return false;

            var output = string.Empty;
            var error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{envHelper.GetSolutionDir()}\" && \"{envHelper.GetGit()}\" {commands}",
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

        public static void StartTortoiseGitProc(EnvHelper envHelper, string args)
        {
            var tortoiseGitProc = envHelper.GetTortoiseGitProc();
            try
            {
                var task = Task.Run(() => Process.Start(tortoiseGitProc, args));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, $"{tortoiseGitProc} not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Execute a Git command and return the output
        /// </summary>
        /// <param name="commands">Git command to be executed</param>
        /// <returns>Git output</returns>
        public static string StartProcessGitResult(EnvHelper envHelper, string commands)
        {
            if (string.IsNullOrEmpty(envHelper.GetSolutionDir())) return string.Empty;

            var output = string.Empty;
            var error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{envHelper.GetSolutionDir()}\" && \"{envHelper.GetGit()}\" {commands}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output += proc.StandardOutput.ReadLine() + ";";
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }

            return string.IsNullOrEmpty(output) ? error : output.TrimEnd(';');
        }

        public static string GitResult(EnvHelper envHelper, string workingDir, string commands)
        {
            var output = string.Empty;
            var error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd /D \"{workingDir}\" && \"{envHelper.GetGit()}\" {commands}",
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

        public static void Start(string application)
        {
            Process.Start(application);
        }

        public static void Start(string application, string arguments)
        {
            Process.Start(application, arguments);
        }

        public static Process StartProcessGui(DTE dte, EnvHelper envHelper, string application, string args, string title, string branchName = "", 
            OutputBox outputBox = null, OptionPageGrid options = null, string pushCommand = "")
        {
            var dialogResult = DialogResult.OK;
            if (!StartProcessGit(envHelper, "config user.name") || !StartProcessGit(envHelper, "config user.email"))
            {
                dialogResult = new Credentials(envHelper).ShowDialog();
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
                            WorkingDirectory = envHelper.GetSolutionDir()
                        },
                        EnableRaisingEvents = true
                    };
                    process.Exited += process_Exited;
                    process.OutputDataReceived += OutputDataHandler;
                    process.ErrorDataReceived += OutputDataHandler;

                    if (outputBox == null)
                    {
                        _outputBox = new OutputBox(branchName, options, pushCommand, dte, envHelper);
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
                    MessageBox.Show(e.Message, $"'{application}' not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return null;
        }

        private static void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data)) return;
            var process = sendingProcess as Process;
            if (process == null) return;

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
            var process = sender as Process;
            if (process == null) return;        

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
    }
}
