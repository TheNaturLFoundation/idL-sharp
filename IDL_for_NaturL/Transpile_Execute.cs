using System;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Path = System.IO.Path;
using ICSharpCode.AvalonEdit;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        // Attributes declared for process handling (if the user wants to cut one)
        private Process _process = new Process();
        private Process _processPython = new Process();
        private bool _processPythonRunning;
        //Function in order to quote paths as the cmd doesn't understand what a path with spaces is

        private void Transpile(object sender, RoutedEventArgs routedEventArgs)
        {
            if (string.IsNullOrEmpty(((TextEditor) FindName("CodeBox" + _currenttabId)).Text)) return;
            string arguments = language switch
            {
                IDL_for_NaturL.Language.French => "--language french",
                IDL_for_NaturL.Language.English => "--language english",
                _ => throw new ArgumentOutOfRangeException()
            };

            _process = new Process
            {
                StartInfo =
                {
                    FileName = "resources/naturL.exe",
                    WorkingDirectory = "resources/",
                    EnvironmentVariables = { ["NATURLPATH"] =
                        Path.GetFullPath("resources")},
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
            StreamReader outputreader = _process.StandardOutput;
            inputWriter.Write(((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
            inputWriter.Close();
            string error = reader.ReadLine();
            string output = outputreader.ReadToEnd();
            if (error == null)
            {
                ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.ForestGreen;
                if (FrenchBox.IsChecked)
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = "Transpilation réussie";
                }
                else
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = "Transpilation succeeded";
                }

                ((TextEditor) FindName("python" + _currenttabId)).Text = output;
            }
            else
            {
                if (error[0] == 'W')
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Orange;
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = error;
                    ((TextEditor) FindName("python" + _currenttabId)).Text = output;
                }
                else
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Red;
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = error;
                }
            }
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextEditor) FindName("CodeBox" + _currenttabId)).Text)) return;
            string arguments = language switch
            {
                IDL_for_NaturL.Language.French => "--language french",
                IDL_for_NaturL.Language.English => "--language english",
                _ => throw new ArgumentOutOfRangeException()
            };
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "resources/naturL.exe",
                    WorkingDirectory = "resources/",
                    Arguments = arguments,
                    EnvironmentVariables = {["NATURLPATH"] = Path.GetFullPath("resources")},
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            _process.Start();
            StreamWriter inputWriter = _process.StandardInput;
            StreamReader reader = _process.StandardError;
            StreamReader outputreader = _process.StandardOutput;
            inputWriter.Write(((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
            inputWriter.Close();
            string errorTranspile = reader.ReadLine();
            string outputTranspile = outputreader.ReadToEnd();
            if (errorTranspile == null)
            {
                _processPython = new Process
                {
                    StartInfo =
                    {
                        FileName = "python",
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
                ((TextEditor) FindName("STD" + _currenttabId)).Clear();
                _processPython.Start();
                _processPythonRunning = true;
                inputWriter = _processPython.StandardInput;
                inputWriter.Write(outputTranspile);
                inputWriter.Close();
                _processPython.OutputDataReceived += (sender2, e2) =>
                {
                    _processPython.Suspend();
                    Thread.Sleep(10);
                    _processPython.Resume();
                    Dispatcher.Invoke(() =>
                    {
                        if (!_processPythonRunning) return;
                        ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Black;
                        ((TextEditor) FindName("STD" + _currenttabId)).Text += e2.Data + '\n';
                        ((TextEditor) FindName("STD" + _currenttabId)).ScrollToEnd();
                    });
                };
                _processPython.ErrorDataReceived += (sender1, e1) => Dispatcher.Invoke(() =>
                {
                    if (string.IsNullOrEmpty(e1.Data) || _processPythonRunning) return;
                    ((TextEditor) FindName("STD" + _currenttabId)).Foreground = e1.Data[0] == 'W' ? Brushes.Orange : Brushes.Red;
                    ((TextEditor) FindName("STD" + _currenttabId)).Text += e1.Data;

                });
                _processPython.Exited += (sender3, e3) => _processPythonRunning = false;
                _processPython.BeginOutputReadLine();
                _processPython.BeginErrorReadLine();
            }
            else
            {
                ((TextEditor) FindName("STD" + _currenttabId)).Foreground = errorTranspile[0] == 'W' ? Brushes.Orange : Brushes.Red;
                ((TextEditor) FindName("STD" + _currenttabId)).Text = errorTranspile;
            }
        }

        public void Cancel_Process(object sender, RoutedEventArgs e)
        {
            _processPython.Kill();
            _processPython.Dispose();
            ((TextEditor) FindName("STD" + _currenttabId)).Text = language switch
            {
                IDL_for_NaturL.Language.English => ("Process finished with exit code -1"),
                IDL_for_NaturL.Language.French =>
                ("Processus terminé avec le code de retour -1"),
                _ => throw new ArgumentOutOfRangeException()
            };
            _processPythonRunning = false;
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
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
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