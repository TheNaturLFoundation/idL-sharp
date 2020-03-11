using System;
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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;


namespace IDL_for_NaturL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //Constants in order to determine if the data is dirty or know
        private string _firstData = "";
        private bool _isSaved;
        private bool _isFileSelected;
        private string _file = "";
        
        public MainWindow()
        {
            InitializeComponent();
            string path = File.ReadAllText("../../../ressources/lastfile.txt");
            if (path != "")
            {
                CodeBox.Text = File.ReadAllText(path);
                _file = path;
                _isFileSelected = true;
                _firstData = CodeBox.Text;
            }

            /*var myAssembly = Assembly.GetExecutingAssembly(); 
            using Stream s = myAssembly.GetManifestResourceStream("MyHighlighting.xshd");
            using XmlTextReader reader = new XmlTextReader(s);
            CodeBox.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);*/
        }

        //THESE ARE THE METHODS THAT MANAGE THE INTERFACE BASIC COMMANDS-------------------------
        private bool DataChanged()
        {
            if (_firstData != CodeBox.Text)
            {
                _isSaved = false;
            }

            return (_firstData != CodeBox.Text);


        }

        // This function refers to the "Open" button in the toolbar, opens the file dialog and asks the user the file to open
        // Content of the opened file is then showed in the codebox of idl
        private void Open_Click()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "nl files (*.ntl)|*.ntl|Text files (*.txt)|*.txt"
            };
            if (DataChanged() && !_isSaved)
            {
                MessageBoxResult result =
                    MessageBox.Show("The file has been modified \n Would you like to save your changes ?", "Data App",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click();
                }
            }

            if (openFileDialog.ShowDialog() == true)
            {
                _file = openFileDialog.FileName;
                var text = File.ReadAllText(_file);
                CodeBox.Text = text;
                _firstData = text;
                Tab1.Header = Path.GetFileNameWithoutExtension(_file);
                _isSaved = false;
                _isFileSelected = true;
            }
        }

        /*
         Do not touch at the moment, it is a method for opening a new window at the moment it is an input box
         Can be useful for a lot of stuff for next presentation
         private void NewWindow(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new InputWindow((arg) => { this._file = arg; });
            inputWindow.Show();
        }*/

        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (DataChanged() && !_isSaved)
            {
                MessageBoxResult result = MessageBox.Show(
                    "The file has been modified \n Would you like to save your changes ?",
                    "Data App", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click();
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "nl files (*.ntl)|*.ntl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _file = saveFileDialog.FileName;
                if (!_file.EndsWith(".ntl"))
                    _file += ".ntl";
                File.Create(_file);
                _isSaved = false;
                _isFileSelected = true;
                Tab1.Header = Path.GetFileNameWithoutExtension(_file);
                CodeBox.Text = "";
            }
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            MainWindow newwindow = new MainWindow();
            newwindow.Show();
        }
        
        // This function refers to the "Save" button in the toolbar, opens the file dialog and asks the user the file to overwrite
        private void Save_Click()
        {
            if (!_isFileSelected)
            {
                Save_AsClick();
            }
            else
            {
                File.WriteAllText(_file, CodeBox.Text);
                _isSaved = true;
                //var text = File.ReadAllText(_file);
                _firstData = CodeBox.Text; 
            }
        }

        private void Save_AsClick()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "ntl files (*.ntl)|*.ntl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() != true) return;
            _file = saveFileDialog.FileName;
            if (!_file.Contains(".ntl"))
                _file += ".ntl";
            File.WriteAllText(_file, CodeBox.Text);
            _isSaved = true;
            _isFileSelected = true;
            Tab1.Header = Path.GetFileName(_file);
            string text = File.ReadAllText(_file);
            _firstData = text.ToString();
        }

        // This function refers to the event handler "IDL_Closing" in "Window" attributes,
        // Handles the window closing, asks whether the user wants to save his file before closing.
        private void IDL_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText("../../../ressources/lastfile.txt",_file);
            if (DataChanged() && !_isSaved)
            {
                string msg = "Do you want to save your changes ?\n";
                MessageBoxResult result = MessageBox.Show(msg, "Data App", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // If user want to close and save, cancel closure
                    Save_Click();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Transpile(object sender, RoutedEventArgs routedEventArgs)
        {
            if (DataChanged() && !_isSaved)
            {
                Save_Click();
            }

            if (CodeBox.Text != "")
            {
                string path = Path.GetFullPath(_file);
                string python_file = Path.ChangeExtension(path, ".py");
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "../../../ressources/naturL.exe",
                        Arguments = path + " " + python_file,
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
                    STD.Text = "Transpilation succeded";
                    python.Text = File.ReadAllText(python_file);
                }
                else
                {
                    STD.Text = error;
                }
            }
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            Transpile(sender,e);
            string path = Path.GetFullPath(_file); 
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = "python",
                    Arguments = Path.ChangeExtension(path, ".py"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.EnableRaisingEvents = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            StreamReader errorReader = process.StandardError;
            string error = errorReader.ReadToEnd();
            StreamReader outputReader = process.StandardOutput;
            string output = outputReader.ReadToEnd();
            STD.Text = error;
            STD.Text += output;
            
        }
        //-----------------------------------------------------------------------------------------

        //These are the basic commands

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

        //aled elie.
        #region NewTab
        private void NewTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string savedTabControl = XamlWriter.Save(this.FindName("Tab1"));

            StringReader stringReader = new StringReader(savedTabControl);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            TabItem newTabControl = (TabItem)XamlReader.Load(xmlReader);
            newTabControl.Name = "Tab2";
            //renommer les trucs quoi
            ((TabControl)FindName("TabControl")).Items.Add(newTabControl);
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
                new KeyGesture(Key.N,ModifierKeys.Control | ModifierKeys.Shift)
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
    }
    

    #endregion
}


