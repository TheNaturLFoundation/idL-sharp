using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Path = System.IO.Path;
using ICSharpCode.AvalonEdit;
using System.Text;
using System.Windows.Media;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        //Function in order to quote paths as the cmd doesn't understand what a path with spaces is
        private string Quote(string toBeQuoted)
        {
            if (toBeQuoted != null)
            {
                return '"' + toBeQuoted + '"';
            }

            return null;
        }
        private bool Transpile(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!string.IsNullOrEmpty(((TextEditor) FindName("CodeBox" + _currenttabId)).Text))
            {
                string arguments = "";
                if ((bool) FrenchBox.IsChecked)
                {
                    arguments = "--language french";
                }

                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "../../../ressources/naturL.exe",
                        WorkingDirectory = "../../../ressources/",
                        EnvironmentVariables = { ["NATURLPATH"] =
                            Path.GetFullPath("../../../ressources")},
                        Arguments = arguments,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true
                    }
                };
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                StreamWriter inputWriter = process.StandardInput;
                StreamReader reader = process.StandardError;
                StreamReader outputreader = process.StandardOutput;
                Console.WriteLine(outputreader.CurrentEncoding);
                Console.WriteLine("petit ");
                inputWriter.Write(((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
                Console.WriteLine("yop");
                inputWriter.Close();
                process.WaitForExit();
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

                return true;
            }

            return false;
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(((TextEditor) FindName("CodeBox" + _currenttabId)).Text))
            {
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "../../../ressources/naturL.exe",
                        WorkingDirectory = "../../../ressources/",
                        EnvironmentVariables = { ["NATURLPATH"] =
                            Path.GetFullPath("../../../ressources")},
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };
                //process.StartInfo.EnvironmentVariables["NATURLPATH"] =
                //    Path.GetFullPath("../../../ressources/naturL.exe");
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                StreamWriter inputWriter = process.StandardInput;
                StreamReader reader = process.StandardError;
                StreamReader outputreader = process.StandardOutput;
                inputWriter.Write(((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
                inputWriter.Close();
                process.WaitForExit();
                string errorTranspile = reader.ReadLine();
                string outputTranspile = outputreader.ReadToEnd();
                if (errorTranspile == null)
                {
                    Process processPython = new Process
                    {
                        StartInfo =
                        {
                            FileName = "python",
                            UseShellExecute = false,
                            StandardOutputEncoding = Encoding.UTF8,
                            StandardErrorEncoding = Encoding.UTF8,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };
                    processPython.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processPython.EnableRaisingEvents = true;
                    processPython.Start();
                    inputWriter = processPython.StandardInput;
                    inputWriter.Write(outputTranspile);
                    inputWriter.Close();
                    processPython.WaitForExit();
                    StreamReader errorReader = processPython.StandardError;
                    string error = errorReader.ReadToEnd();
                    StreamReader outputReader = processPython.StandardOutput;
                    string output = outputReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        if (error[0] == 'W')
                        {
                            ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Orange;
                        }
                        else
                        {
                            ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Red;
                        }
                    }
                    else
                    {
                        ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Black;
                    }
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = error;
                    ((TextEditor) FindName("STD" + _currenttabId)).Text += output;
                }
                else
                {
                    if (errorTranspile[0] == 'W')
                    {
                        ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Orange;
                    }
                    else
                    {
                        ((TextEditor) FindName("STD" + _currenttabId)).Foreground = Brushes.Red;
                    }
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = errorTranspile;
                }
            }

        }
    }
}