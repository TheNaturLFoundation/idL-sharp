using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using Path = System.IO.Path;
using System.Windows.Controls;
using Dragablz;
using ICSharpCode.AvalonEdit;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
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
        
        public bool Open_Click(string uri = null)
        {
            if (uri != null)
            {
                int fileKey = IsFileOpen(uri);
                if (fileKey != -1)
                {
                    // Select the openend file
                    Dispatcher.Invoke(() => TabControl.SelectedIndex = fileKey);
                    Dispatcher.Invoke(() => _lastFocusedTextEditor =
                        (TextEditor) FindName("CodeBox" + _currenttabId));
                    return false;
                }
                // Open the file according to the uri
                uri = uri.Replace("file://", "");
                OpenFile(uri);
                return true;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "nl files (*.ntl)|*.ntl|Text files (*.txt)|*.txt| cs files (*.cs)|*.cs"
            };
            if (openFileDialog.ShowDialog() != true) return false;
            string filename = openFileDialog.FileName;
            int filekey = IsFileOpen(filename);
            if (filekey == -1)
            {
                OpenFile(filename);
            }
            else
            {
                Dispatcher.Invoke(() => TabControl.SelectedIndex = filekey);
                Dispatcher.Invoke(() => _lastFocusedTextEditor = 
                    (TextEditor) FindName("CodeBox" + _currenttabId));
            }

            return true;
        }

        public void OpenFile(string path)
        {
            NewTabItems(++_tabInt, path);
            var text = File.ReadAllText(_currentTabHandler._file);
            ((TextEditor) FindName("CodeBox" + _currenttabId)).Text = text;
            _currentTabHandler._firstData = text;
            ((TabItem) FindName("Tab" + _currenttabId)).Header =
                Path.GetFileNameWithoutExtension(_currentTabHandler._file);
        }

        private int IsFileOpen(string uri)
        {
            int index = 0;
            if (uri.StartsWith("file://"))
            {
                uri = uri.Replace("file://","");
            }
            foreach (var element in Dispatcher.Invoke(() => tabAttributes))
            {
                if (uri == element.Value._file || uri == element.Value.playground)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        
        public bool NewFile(object sender, RoutedEventArgs e)
        {
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
                NewTabItems(++_tabInt, filename);
                _currentTabHandler._isSaved = false;
                _currentTabHandler._isFileSelected = true;
                ((TabItem) FindName("Tab" + _currenttabId)).Header =
                    Path.GetFileNameWithoutExtension(_currentTabHandler._file);
                ((TextEditor) FindName("CodeBox" + _currenttabId)).Text = "";
                return true;
            }

            return false;
        }
        
        private void NewWindow(object sender, RoutedEventArgs e)
        {
            MainWindow newwindow = new MainWindow();
            newwindow.Show();
        }
        
        //Functiom in order to write all text in a file
        private void WriteAllTextSafe()
        {
            if (!string.IsNullOrEmpty(_currentTabHandler._file))
            {
                File.WriteAllText(_currentTabHandler._file, ((TextEditor) FindName("CodeBox" + _currenttabId)).Text);
            }
        }
        
        // This function refers to the "Save" button in the toolbar, opens the file dialog and asks the user the file to overwrite
        private bool Save_Click()
        {
            if (!_currentTabHandler._isFileSelected)
            {
                return Save_AsClick();
            }

            WriteAllTextSafe();
            _currentTabHandler._isSaved = true;
            _currentTabHandler._firstData = ((TextEditor) FindName("CodeBox" + _currenttabId)).Text;
            return true;
        }
        

        private bool Save_AsClick()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "ntl files (*.ntl)|*.ntl*|Text files (*.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() != true) return false;
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
            _currentTabHandler.playground = null;
            return true;
        }
    }
}