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

namespace IDL_for_NaturL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Constants in order to determine if the data is dirty or know
        //Can improve it by saving the string of the opened file and comparing it to the one we want to save
        //This one detects any keyboard stroke
        bool DataChanged = false;
        public bool IsSaved;
        public string file = "";
        public bool newfile = false;
        public MainWindow()
        {
            //InitializeComponent();
        }

        //THESE ARE THE METHODS THAT MANAGE THE INTERFACE BASIC COMMANDS-------------------------

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "nl files (*.nl)|*.nl*|Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                CodeBox.Text = "";
                file = openFileDialog.SafeFileName;
                var filestream = openFileDialog.OpenFile();
                var Table = File.ReadAllText(file);
                CodeBox.Text += Table;
                Tab1.Header = System.IO.Path.GetFileName(file);
                IsSaved = false;
            }
            //System.IO.File.Create(System.IO.Path.GetFullPath(nameoffile)); Ancient version, saves file in path with the .exe program
            //We don't want that, we want to open file explorer
        }


        private void Save_Click(object sender, RoutedEventArgs e = null)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "nl files (*.nl)|*.nl*|Text files (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {

                File.WriteAllText(saveFileDialog.FileName, CodeBox.Text);
                IsSaved = true;
                file = saveFileDialog.FileName;
                Tab1.Header = System.IO.Path.GetFileName(file);
            }

        }

        private void IDL_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataChanged && !IsSaved)
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
        private void inputF_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                newfile = true;
                if (inputF.Text.Contains(".nl"))
                {
                    var myfile = File.Create(inputF.Text);
                    myfile.Close();

                }
                else
                {
                    var myfile = File.Create(inputF.Text);
                    myfile.Close();
                }
                
            }
            else if(e.Key == Key.Escape)
            {
                inputF.Text = "Input file name";
                newfile = false;
            }
            if (newfile)
            {
                Open_Click(sender, e);
            }
        }
        public void New_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                inputF.Text = "Input file name";
            }
        }
        //Faire une classe pour le new, qui ouvre une nouvelle fenetre

        //-----------------------------------------------------------------------------------------

        //THESE ARE THE COMMANDS BINDED

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


        //------------------------------------------------------------------------------------------
    }

    #region CustomClasses
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
    }
    #endregion

}