using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using Dragablz;
using ICSharpCode.TextEditor.Document;
using HighlightingManager = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace IDL_for_NaturL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //Constants in order to determine if the data is dirty or know
        //private string _firstData = "";
        //private bool _isSaved;
        //private bool _isFileSelected;
        //private string _file = "";
        private static int _tabInt;
        private int _currentTab;
        private string _currenttabId = "0";
        private string tabitem;
        private static Dictionary<string, TabHandling> attributes = new Dictionary<string, TabHandling>();
        private TabHandling _currentTabHandler;
        private IHighlightingDefinition _highlightingDefinition;
        private bool _isFullSize;
        private double clickPosition;

        private class TabHandling
        {
            public TabHandling(string file, bool isSaved = false)
            {
                _isFileSelected = !string.IsNullOrEmpty(file);
                _firstData = file == null ? "" : File.ReadAllText(file);
                _isSaved = isSaved;
                _file = file;
            }
            

            //public TabHandling()
            public bool _isFileSelected { get; set; }
            public string _firstData { get; set; }
            public bool _isSaved { get; set; }
            public string _file { get; set; }

            public override string ToString() =>
                $"(Is File Selected : {_isFileSelected}, FirstData : {"first_data"}, IsSaved : {_isSaved}, File : {_file})";
        }


        public MainWindow()
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            InitializeComponent();
            Console.WriteLine("Initialized MainWindow");
            TextEditor textEditor =
                (TextEditor) ((Grid) ((TabItem) FindName("Tab_id_")).FindName("grid_codebox")).Children[0];
            XmlTextReader reader = new XmlTextReader("../../../naturl_coloration.xshd");
            _highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            textEditor.SyntaxHighlighting = _highlightingDefinition;
            reader.Close();

            //attributes = new Dictionary<string, TabHandling>();
            string[] paths = File.ReadAllLines("../../../ressources/lastfiles.txt");
            tabitem = XamlWriter.Save(this.FindName("Tab_id_"));
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(0);
            if (paths.Length == 0)
            {
                NewTabItems(_tabInt++, null);
                Console.WriteLine("isfileselected " + _currentTabHandler._isFileSelected);
            }
            else
            {
                foreach (string path in paths)
                {
                    NewTabItems(_tabInt++, path);
                }
            }
        }


        public void RemoveTab(int tabindex)
        {
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(tabindex);
        }

        private void NewTabItems(int n, string path)
        {
            Console.WriteLine("NewTabItems" + n);
            StringReader stringReader = new StringReader(tabitem.Replace("_id_", n.ToString()));
            XmlReader xmlReader = XmlReader.Create(stringReader);
            TabItem newTabControl = (TabItem) XamlReader.Load(xmlReader);
            RegisterName("Tab" + n, newTabControl);
            ((TabablzControl) FindName("TabControl"))?.Items.Add(newTabControl);
            RegisterName("CodeBox" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName("grid_codebox")).Children[0]);
            RegisterName("python" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName("python_grid")).Children[0]);
            RegisterName("STD" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName("python_grid")).Children[3]);

            TabHandling tabHandling = new TabHandling(path);
            attributes.Add(n.ToString(), tabHandling);
            ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName("grid_codebox")).Children[0])
                .SyntaxHighlighting = _highlightingDefinition;

            TabControl.SelectedIndex = ((TabablzControl) FindName("TabControl")).Items.Count - 1;
            if (!attributes.TryGetValue(_currenttabId, out _currentTabHandler))
            {
                Console.WriteLine("Warning in CurrenttabHandler (On selection changed)");
            }

            if (path != null)
            {
                string s = File.ReadAllText(path);
                ((TextEditor) FindName("CodeBox" + n)).Text = s;
                ((TabItem) FindName("Tab" + n)).Header = Path.GetFileNameWithoutExtension(tabHandling._file);
                tabHandling._firstData = s;
                tabHandling._isFileSelected = true;
                tabHandling._isSaved = true;
            }
        }

        //THESE ARE THE METHODS THAT MANAGE THE INTERFACE BASIC COMMANDS-------------------------
        private bool DataChanged()
        {
            if (_currentTabHandler._firstData != ((TextEditor) FindName("CodeBox" + _currenttabId)).Text)
            {
                _currentTabHandler._isSaved = false;
            }

            return _currentTabHandler._firstData != ((TextEditor) FindName("CodeBox" + _currenttabId)).Text;
        }

        // This function refers to the "Open" button in the toolbar, opens the file dialog and asks the user the file to open
        // Content of the opened file is then showed in the codebox of idl
        public void Open_Click()
        {
            Console.WriteLine("Open_Click");
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "nl files (*.ntl)|*.ntl|Text files (*.txt)|*.txt| cs files (*.cs)|*.cs"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                NewTabItems(_tabInt++, filename);
                var text = File.ReadAllText(_currentTabHandler._file);
                ((TextEditor) FindName("CodeBox" + _currenttabId)).Text = text;
                _currentTabHandler._firstData = text;
                ((TabItem) FindName("Tab" + _currenttabId)).Header =
                    Path.GetFileNameWithoutExtension(_currentTabHandler._file);
            }
        }

        /*
         Do not touch at the moment, it is a method for opening a new window at the moment it is an input box
         Can be useful for a lot of stuff for next presentation
         private void NewWindow(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new InputWindow((arg) => { this._file = arg;TabControl_OnSelectionChangedShow();
        }*/

        public void NewFile(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("NewFile");
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "nl files (*.ntl)|*.ntl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                if (!filename.EndsWith(".ntl"))
                    filename += ".ntl";
                Stream myfile = File.Create(filename);
                myfile.Close();
                NewTabItems(_tabInt++, filename);
                _currentTabHandler._isSaved = false;
                _currentTabHandler._isFileSelected = true;
                ((TabItem) FindName("Tab" + _currenttabId)).Header =
                    Path.GetFileNameWithoutExtension(_currentTabHandler._file);
                ((TextEditor) FindName("CodeBox" + _currenttabId)).Text = "";
            }
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            MainWindow newwindow = new MainWindow();
            newwindow.Show();
        }

        //Functiom in order to write all text in a file
        private void WriteAllTextSafe()
        {
            if (!string.IsNullOrEmpty(_currentTabHandler?._file))
            {
                File.WriteAllText(_currentTabHandler._file, ((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
            }
            else
            {
                NewFile(null, null);
            }
        }

        // This function refers to the "Save" button in the toolbar, opens the file dialog and asks the user the file to overwrite
        private void Save_Click()
        {
            if (!_currentTabHandler._isFileSelected)
            {
                Save_AsClick();
            }
            else
            {
                WriteAllTextSafe();
                _currentTabHandler._isSaved = true;
                _currentTabHandler._firstData = ((TextEditor) FindName("CodeBox" + _currenttabId)).Text;
            }
        }

        private void Save_AsClick()
        {
            Console.WriteLine("Save_AsClick");
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "ntl files (*.ntl)|*.ntl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() != true) return;
            _currentTabHandler._file = saveFileDialog.FileName;
            if (!_currentTabHandler._file.Contains(".ntl"))
                _currentTabHandler._file += ".ntl";
            WriteAllTextSafe();
            _currentTabHandler._isSaved = true;
            _currentTabHandler._isFileSelected = true;
            ((TabItem) FindName("Tab" + _currenttabId)).Header =
                Path.GetFileNameWithoutExtension(_currentTabHandler._file);
            string text = File.ReadAllText(_currentTabHandler._file);
            _currentTabHandler._firstData = text.ToString();
        }

        // This function refers to the event handler "IDL_Closing" in "Window" attributes,
        // Handles the window closing, asks whether the user wants to save his file before closing.

        private MessageBoxResult messageOnClose(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Data App", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            return result;
        }

        private void IDL_Closing(object sender, RoutedEventArgs cancelEventArgs)
        {
            string paths = "";
            bool cancelled = false;
            foreach (TabItem item in ((TabablzControl) FindName("TabControl")).Items)
            {
                _currenttabId = item.Name.Replace("Tab", "");
                if (!attributes.TryGetValue(_currenttabId, out _currentTabHandler))
                {
                    Console.WriteLine("Warning in CurrenttabHandler (On selection changed)");
                }

                if (!_currentTabHandler._isFileSelected && DataChanged())
                {
                    MessageBoxResult result = messageOnClose("Your changes on the file: " + item.Header +
                                                             " are not saved. \n Would you like to save them?");
                    if (result == MessageBoxResult.Yes)
                    {
                        Save_Click();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        continue;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        cancelled = true;
                        break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(_currentTabHandler._file))
                    {
                        File.WriteAllText(_currentTabHandler._file,
                            ((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
                        paths += Path.GetFullPath(_currentTabHandler._file) + '\n';
                    }
                }
            }

            File.WriteAllText("../../../ressources/lastfiles.txt",
                paths);
            if (!cancelled)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_Resize(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        // Function in order to unregister previous instances (used for closing a tab)
        private void UnregisterNamesAndRemove()
        {
            Console.WriteLine("Remove " + _currenttabId);
            UnregisterName("Tab" + _currenttabId);
            UnregisterName("CodeBox" + _currenttabId);
            UnregisterName("python" + _currenttabId);
            UnregisterName("STD" + _currenttabId);
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(_currentTab);
            TabControl.SelectedIndex = ((TabablzControl) FindName("TabControl")).Items.Count - 1;
            //attributes.Remove(_currenttabId);
        }

        private void CloseTab()
        {
            if (DataChanged() && !_currentTabHandler._isSaved)
            {
                string msg = "Do you want to save your changes ?\n";
                MessageBoxResult result = MessageBox.Show(msg, "Data App", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click();
                    UnregisterNamesAndRemove();
                }
                else if (result == MessageBoxResult.No)
                {
                    UnregisterNamesAndRemove();
                }
            }
            else
            {
                UnregisterNamesAndRemove();
            }
        }

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
            Console.WriteLine(_currentTabHandler._isFileSelected + " " + _currentTabHandler._isSaved + " " +
                              DataChanged());
            if (!_currentTabHandler._isFileSelected || !_currentTabHandler._isSaved || DataChanged())
            {
                Save_Click();
            }

            if (((TextEditor) FindName("CodeBox" + _currenttabId)).Text != "")
            {
                string path = Path.GetFullPath(_currentTabHandler._file);
                string pythonFile = Path.ChangeExtension(path, ".py");
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "../../../ressources/naturL.exe",
                        Arguments = "--input " + Quote(path) + " --output " + Quote(pythonFile),
                        UseShellExecute = false,
                        RedirectStandardError = true
                    }
                };
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                StreamReader reader = process.StandardError;
                string error = reader.ReadLine();

                if (error == null)
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = "Transpilation succeded";
                    ((TextEditor) FindName("python" + _currenttabId)).Text = File.ReadAllText(pythonFile);
                }
                else
                {
                    ((TextEditor) FindName("STD" + _currenttabId)).Text = error;
                }

                return true;
            }

            return false;
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            if (Transpile(sender, e))
            {
                string path = Path.GetFullPath(_currentTabHandler._file);
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "python",
                        Arguments = Quote(Path.ChangeExtension(path, ".py")),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;
                process.Start();
                process.WaitForExit();
                StreamReader errorReader = process.StandardError;
                string error = errorReader.ReadToEnd();
                StreamReader outputReader = process.StandardOutput;
                string output = outputReader.ReadToEnd();
                ((TextEditor) FindName("STD" + _currenttabId)).Text = error;
                ((TextEditor) FindName("STD" + _currenttabId)).Text += output;
            }
        }

        //-----------------------------------------------------------------------------------------

        //These are the basic commands

        #region CommandsExecution

        #region ExitCommand

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region SaveCommand

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_Click();
        }

        #endregion

        #region OpenCommand

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Click();
        }

        #endregion

        #region SaveAsCommand

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_AsClick();
        }

        #endregion

        #region Transpile and Execute

        private void TranspileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TranspileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Transpile(sender, e);
        }

        private void ExecuteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecuteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Execute(sender, e);
        }

        #endregion

        #region NewTab

        private void NewTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewTabItems(_tabInt++, null);
        }

        #endregion

        #region NewWindow

        private void NewWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewWindow(sender, e);
        }

        #endregion

        #region New File

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewFile(sender, e);
        }

        #endregion

        private void CloseTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((TabablzControl) FindName("TabControl")).Items.Count > 1;
        }

        private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseTab();
        }

        private void SettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            // Used .Show() for debugging there but should use .ShowDialog()
            settingsWindow.Show();
        }

        private void DebugCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DebugCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (KeyValuePair<string,TabHandling> att in attributes)
            {
                Console.WriteLine("Key is: " + att.Key + " Value is: "+ att.Value);
            }
            Console.WriteLine("CurrentTabHandler: "+ _currentTabHandler);
            Console.WriteLine("Size: " + attributes.Count);
        }
        
        
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 50 && e.ClickCount == 1)
            {
                clickPosition = e.GetPosition(this).Y;
            }
            else
            {
                clickPosition = -1;
            }
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (position.Y < 50)
            {
                this.DragMove();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            double yVel = e.GetPosition(this).Y - clickPosition;
            if (yVel > 0 && clickPosition > 0 && WindowState == WindowState.Maximized && e.ClickCount == 1)
            {
                WindowState = WindowState.Normal;
            }
            else
                clickPosition = -1;
        }

        private void Double_Click(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            double y = p.Y;
            if (y < 50)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }


        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((TabablzControl) e.Source).SelectedIndex == -1)
            {
                ((TabablzControl) e.Source).SelectedIndex = 0;
            }
            Console.WriteLine("Added items " + e.AddedItems.Count+", Removed items " + e.RemovedItems.Count);
            if (e.AddedItems.Count != 0)
            {
                var source = (TabItem) e.AddedItems[0];
                _currentTab = ((TabablzControl) e.Source).SelectedIndex;
                _currenttabId = source.Name.Replace("Tab", "");
                Console.WriteLine("Current tab id " + _currenttabId);
                if (!attributes.TryGetValue(_currenttabId, out _currentTabHandler))
                {
                    Console.WriteLine("Warning in CurrenttabHandler (On selection changed)");
                }
            }
            else
            {
                var source = (TabItem) e.RemovedItems[0];
                string i = source.Name.Replace("Tab", "");
                attributes.Remove(i);
                
            }
        }

        #endregion

        private void DragEnter(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Drag enter");
        }
        
        private void DragLeave(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Drag leave");
        }
        
        private void DraggingChanged(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Dragging changed");
        }
        
        private void DragOver(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Drag over");
        }
    }

    //------------------------------------------------------------------------------------------


    #region CustomCommands

    // This class CustomCommands allows us to create custom commands with keyboard shortcuts
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

        //Define more commands here, just like the one above
        public static readonly RoutedUICommand Save = new RoutedUICommand
        (
            "Save",
            "Save",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Open = new RoutedUICommand
        (
            "Open",
            "Open",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand NewTab = new RoutedUICommand
        (
            "NewTab",
            "NewTab",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand SaveAs = new RoutedUICommand
        (
            "Save_As",
            "Save_As",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand Transpile = new RoutedUICommand
        (
            "Transpile",
            "Transpile",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Execute = new RoutedUICommand
        (
            "Execute",
            "Execute",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5)
            }
        );

        public static readonly RoutedUICommand NewWindow = new RoutedUICommand
        (
            "NewWindow",
            "NewWindow",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );

        public static readonly RoutedUICommand New = new RoutedUICommand
        (
            "New",
            "New",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand CloseTab = new RoutedUICommand
        (
            "CloseTab",
            "CloseTab",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.W, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Settings = new RoutedUICommand
        (
            "Settings",
            "Settings",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Alt)
            }
        );
        
        public static readonly RoutedUICommand Debug = new RoutedUICommand
        (
            "Debug",
            "Debug",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            }
        );
    }

    #endregion
}