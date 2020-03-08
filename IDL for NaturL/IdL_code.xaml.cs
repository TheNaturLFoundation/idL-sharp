using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Microsoft.VisualBasic;

namespace IDL_for_NaturL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Constants in order to determine if the data is dirty or know
        private string firstData = "";
        private bool IsSaved = false;
        private bool IsFileSelected = false;
        private string file = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        //THESE ARE THE METHODS THAT MANAGE THE INTERFACE BASIC COMMANDS-------------------------
        private bool DataChanged(string firstdata, string current)
        {
            return firstdata != current;
        }
        // This function refers to the "Open" button in the toolbar, opens the file dialog and asks the user the file to open
        // Content of the opened file is then showed in the codebox of idl
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "nl files (*.nl)|*.nl*|Text files (*.txt)|*.txt";
            if (DataChanged(firstData, CodeBox.Text) && !IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("The file has been modified \n Would you like to save your changes ?", "Data App", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender,e);
                }
            }
            if (openFileDialog.ShowDialog() == true)
            {
                CodeBox.Text = "";
                file = openFileDialog.FileName;
                var text = File.ReadAllText(file);
                CodeBox.Text += text;
                firstData = text;
                Tab1.Header = System.IO.Path.GetFileName(file);
                IsSaved = false;
                IsFileSelected = true;
            }
            //System.IO.File.Create(System.IO.Path.GetFullPath(nameoffile)); Ancient version, saves file in path with the .exe program
            //We don't want that, we want to open file explorer
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new InputWindow((arg)=>{ this.file = arg; });
            inputWindow.Show();
        }
        
        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (DataChanged(firstData,CodeBox.Text) && !IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("The file has been modified \n Would you like to save your changes ?", "Data App", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender,e);
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "nl files (*.nl)|*.nl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                file = saveFileDialog.FileName;
                Console.WriteLine(file);
                Console.WriteLine(file);
                if (!File.Exists(file))
                {
                    if (file.Contains(".nl"))
                        File.Create(file);
                    else
                        File.Create(file + ".nl");
                }
                IsSaved = false;
                IsFileSelected = true;
                Tab1.Header = System.IO.Path.GetFileName(file);
            }
        }
        // This function refers to the "Save" button in the toolbar, opens the file dialog and asks the user the file to overwrite
        private void Save_Click(object sender, RoutedEventArgs e = null)
        {
            if (!IsFileSelected)
            {
                Save_AsClick(sender, e);
            }
            else
            {
                File.WriteAllText(file, CodeBox.Text);
            }

        }

        private void Save_AsClick(object sender, RoutedEventArgs e = null)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "nl files (*.nl)|*.nl*|Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, CodeBox.Text);
                IsSaved = true;
                file = saveFileDialog.FileName;
                IsFileSelected = true;
                Tab1.Header = System.IO.Path.GetFileName(file);
            }
            
        }

        // This function refers to the event handler "IDL_Closing" in "Window" attributes,
        // Handles the window closing, asks wether the user wants to save his file before closing.
        private void IDL_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataChanged(firstData,CodeBox.Text) && !IsSaved)
            {
                string msg = "Do you want to save your changes ?\n";
                MessageBoxResult result = MessageBox.Show(msg, "Data App", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // If user want to close and save, cancel closure
                    e.Cancel = true;
                    Save_Click(sender);
                    IsSaved = true;

                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

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
            Save_Click(sender);
        }
        #endregion

        #region OpenCommand
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Click(sender, e);
        }

        #endregion
        
        #region SaveAsCommand
        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_AsClick(sender);
        }
        #endregion
        //------------------------------------------------------------------------------------------
    }

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
                    new KeyGesture(Key.S,ModifierKeys.Control)
               }
           );

        public static readonly RoutedUICommand Open = new RoutedUICommand
           (
               "Open",
               "Open",
               typeof(CustomCommands),
               new InputGestureCollection()
               {
                    new KeyGesture(Key.O,ModifierKeys.Control)
               }
           );

        public static readonly RoutedUICommand New = new RoutedUICommand
           (
               "New",
               "New",
               typeof(CustomCommands),
               new InputGestureCollection()
               {
                    new KeyGesture(Key.N,ModifierKeys.Control)
               }
           );
        public static readonly RoutedUICommand Save_As = new RoutedUICommand
            (
                "Save_As",
                "Save_As",
                typeof(CustomCommands)
                );
    }
    #endregion

}