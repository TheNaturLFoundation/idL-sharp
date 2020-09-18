using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.InteropServices;
using Path = System.IO.Path;
using ICSharpCode.AvalonEdit;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        // Attributes declared for process handling (if the user wants to cut one)
        private Process _process = new Process();
        private Process _processPython = new Process();
        private bool _processPythonRunning;
        //Function in order to quote paths as the cmd doesn't understand what a path with spaces is
        
        private string Quote(string text)
        {
            return '"' + text + '"';
        }
        
        private void Transpile(object sender, RoutedEventArgs routedEventArgs)
        {
            TextEditor STD = (TextEditor) FindName("STD" + _currenttabId);
            TextEditor CodeBox = (TextEditor) FindName("CodeBox" + _currenttabId);
            TextEditor PythonBox = (TextEditor) FindName("python" + _currenttabId);
            STD.TextArea.TextView.LineTransformers.Clear();
            bool readStdin = true;
            if (string.IsNullOrEmpty(CodeBox.Text)) return;
            string arguments = language switch
            {
                IDL_for_NaturL.Language.French => "--language french",
                IDL_for_NaturL.Language.English => "--language english",
                _ => throw new ArgumentOutOfRangeException()
            };
            if (CodeBox.Text.Length > 4096)
            {
                if (_currentTabHandler.playground != null) return;
                Save_Click();
                string file = _currentTabHandler._file;
                string python_file = Path.ChangeExtension(file, ".py");
                File.Delete(python_file);
                arguments += " --input " + Quote(file) + " --output " +
                             Quote(python_file);
                readStdin = false;
            }
            
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "resources/naturL.exe",
                    WorkingDirectory = "resources/",
                    EnvironmentVariables =
                    {
                        ["NATURLPATH"] =
                            Path.GetFullPath("resources")
                    },
                    Arguments = arguments,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.Start();
            StreamWriter inputWriter = _process.StandardInput;
            StreamReader reader = _process.StandardError;
            StreamReader outputreader;
            if (readStdin)
            {
                inputWriter.Write(CodeBox.Text);
                outputreader = _process.StandardOutput;
            }
            else
            {
                _process.WaitForExit();
                outputreader = new StreamReader(
                    Path.ChangeExtension(_currentTabHandler._file,".py"));
            }
            inputWriter.Close();
            string error = reader.ReadToEnd();
            string output = outputreader.ReadToEnd();
            bool containsError = error.Contains("Erreur") || error.Contains("Error");
            if (!containsError)
            {
                STD.Foreground = Brushes.ForestGreen;
                STD.Text = FrenchBox.IsChecked ? "Transpilation réussie" : "Transpilation succeeded";
                PythonBox.Text = output;
            }
            else
            {
                if (error[0] == 'W')
                {
                    STD.Foreground = Brushes.Orange;
                    STD.Text = error;
                    PythonBox.Text = output;
                }
                else
                {
                    STD.Foreground = Brushes.Red;
                    STD.Text = error;
                }
            }
        }
        
        private void Execute(object sender, RoutedEventArgs e)
        {
            TextEditor STD = (TextEditor) FindName("STD" + _currenttabId);
            TextEditor CodeBox = (TextEditor) FindName("CodeBox" + _currenttabId);
            TextEditor PythonBox = (TextEditor) FindName("python" + _currenttabId);
            STD.TextArea.TextView.LineTransformers.Clear();
            bool readStdin = true;
            if (string.IsNullOrEmpty(CodeBox.Text)) return;
            string arguments = language switch
            {
                IDL_for_NaturL.Language.French => "--language french",
                IDL_for_NaturL.Language.English => "--language english",
                _ => throw new ArgumentOutOfRangeException()
            };
            if (CodeBox.Text.Length > 4096)
            {
                if (_currentTabHandler.playground != null) return;
                Save_Click();
                string file = _currentTabHandler._file;
                string python_file = Path.ChangeExtension(file, ".py");
                File.Delete(python_file);
                arguments += " --input " + Quote(file) + " --output " +
                             Quote(python_file);
                readStdin = false;
            }
            
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "resources/naturL.exe",
                    WorkingDirectory = "resources/",
                    EnvironmentVariables =
                    {
                        ["NATURLPATH"] =
                            Path.GetFullPath("resources")
                    },
                    Arguments = arguments,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.Start();
            StreamWriter inputWriter = _process.StandardInput;
            StreamReader reader = _process.StandardError;
            StreamReader outputreader;
            if (readStdin)
            {
                inputWriter.Write(CodeBox.Text);
                outputreader = _process.StandardOutput;
            }
            else
            {
                _process.WaitForExit();
                outputreader = new StreamReader(
                    Path.ChangeExtension(_currentTabHandler._file,".py"));
            }
            inputWriter.Close();
            string error = reader.ReadToEnd();
            string output = outputreader.ReadToEnd();
            bool containsError = error.Contains("Erreur") || error.Contains("Error");
            if (!containsError)
            {
                STD.Foreground = Brushes.Black;
                _processPython = new Process
                {
                    StartInfo =
                    {
                        FileName = "python",
                        WorkingDirectory = "resources/",
                        UseShellExecute = false,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true,
                };

                STD.Clear();
                STD.TextArea.TextView.LineTransformers.Clear();
                _processPython.Start();
                _processPythonRunning = true;
                inputWriter = _processPython.StandardInput;
                inputWriter.Write(output);
                inputWriter.Close();
                string[] warnings = error.Split('\n');
                for (int i = 0; i < warnings.Length-1; i++)
                {
                    STD.TextArea.TextView.LineTransformers.Add(new STDColorizer(i+1,DiagnosticSeverity.Warning));
                    STD.Text += warnings[i];
                }
                _processPython.OutputDataReceived += (sender2, e2) =>
                {
                    _processPython.Suspend();
                    Thread.Sleep(10);
                    _processPython.Resume();
                    Dispatcher.Invoke(() =>
                    {
                        STD.Text += e2.Data + '\n';
                        STD.ScrollToEnd();
                    });
                };
                _processPython.ErrorDataReceived += (sender1, e1) => Dispatcher.Invoke(() =>
                {
                    if (string.IsNullOrEmpty(e1.Data)) return;
                    STD.Foreground = Brushes.Red;
                    STD.Text += e1.Data;
                });
                _processPython.Exited += (sender3, e3) =>
                {
                    _processPythonRunning = false;
                    Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
                };
                _processPython.BeginOutputReadLine();
                _processPython.BeginErrorReadLine();
            }
            else
            {
                STD.Foreground = Brushes.Red;
                STD.Text = error;
            }
        }

        public void Cancel_Process(object sender, RoutedEventArgs e)
        {
            // This is black magic
            Process.GetProcessesByName("Ok so apparently we discovered magic. Like for real" +
                                       "If you try to delete this line, you will get a" +
                                       "problem in the dispay of the STD of idL and we don't understand why ...");
            _processPython.Kill();
            _processPython.Dispose();
            _processPythonRunning = false;
            Dispatcher.InvokeAsync(() =>
            {
                
                ((TextEditor) FindName("STD" + _currenttabId)).Text += language switch
                {
                    IDL_for_NaturL.Language.English => ("Process finished with exit code -1"),
                    IDL_for_NaturL.Language.French =>
                    ("Processus terminé avec le code de retour -1"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }
    }

    public static class ProcessExtension
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern uint TerminateThread(IntPtr thread);

        public static void Suspend(this Process process)
        {
            if (process.HasExited)
            {
                return;
            }

            try
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint) thread.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        break;
                    }
                    SuspendThread(pOpenThread);
                }
            }
            catch (InvalidOperationException)
            {
                // Ignore
            }
        }
        public static void Resume(this Process process)
        {
            try
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint) thread.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        break;
                    }

                    ResumeThread(pOpenThread);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        public static void Terminate(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var openthread = OpenThread(ThreadAccess.TERMINATE, false, (uint) thread.Id);
                if (openthread == IntPtr.Zero)
                    break;
                TerminateThread(openthread);
            }
        }
    }

    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }
}