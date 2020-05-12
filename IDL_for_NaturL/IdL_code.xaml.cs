using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using Dragablz;
using ICSharpCode.AvalonEdit.Search;
using HighlightingManager =
    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

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

        private static Dictionary<string, TabHandling> attributes =
            new Dictionary<string, TabHandling>();

        private TabHandling _currentTabHandler;
        private IHighlightingDefinition _highlightingDefinition;
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
                (TextEditor) ((Grid) ((TabItem) FindName("Tab_id_")).FindName(
                    "grid_codebox")).Children[0];
            XmlTextReader reader =
                new XmlTextReader("../../../naturl_coloration.xshd");
            _highlightingDefinition =
                HighlightingLoader.Load(reader, HighlightingManager.Instance);
            textEditor.SyntaxHighlighting = _highlightingDefinition;
            reader.Close();

            //attributes = new Dictionary<string, TabHandling>();
            string[] paths =
                File.ReadAllLines("../../../ressources/lastfiles.txt");
            tabitem = XamlWriter.Save(this.FindName("Tab_id_"));
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(0);
            if (paths.Length == 0)
            {
                NewTabItems(_tabInt++, null);
                Console.WriteLine("isfileselected " +
                                  _currentTabHandler._isFileSelected);
            }
            else
            {
                foreach (string path in paths)
                {
                    NewTabItems(_tabInt++, path);
                }
            }

            _lastFocusedTextEditor =
                (TextEditor) FindName("CodeBox" + _currenttabId);
        }

        public void RemoveTab(int tabindex)
        {
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(tabindex);
        }

        private void NewTabItems(int n, string path)
        {
            Console.WriteLine("NewTabItems" + n);
            StringReader stringReader =
                new StringReader(tabitem.Replace("_id_", n.ToString()));
            XmlReader xmlReader = XmlReader.Create(stringReader);
            TabItem newTabControl = (TabItem) XamlReader.Load(xmlReader);
            RegisterName("Tab" + n, newTabControl);
            ((TabablzControl) FindName("TabControl"))?.Items.Add(newTabControl);
            RegisterName("CodeBox" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "grid_codebox")).Children[0]);
            RegisterName("python" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[0]);
            RegisterName("STD" + n,
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[3]);
            TabHandling tabHandling = new TabHandling(path);
            attributes.Add(n.ToString(), tabHandling);
            ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "grid_codebox")).Children[0])
                .SyntaxHighlighting = _highlightingDefinition;
            ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[0])
                .SyntaxHighlighting = _highlightingDefinition;


            TabControl.SelectedIndex =
                ((TabablzControl) FindName("TabControl")).Items.Count - 1;
            if (!attributes.TryGetValue(_currenttabId, out _currentTabHandler))
            {
                Console.WriteLine(
                    "Warning in CurrenttabHandler (On selection changed)");
            }

            if (path != null)
            {
                string s = File.ReadAllText(path);
                ((TextEditor) FindName("CodeBox" + n)).Text = s;
                ((TabItem) FindName("Tab" + n)).Header =
                    Path.GetFileNameWithoutExtension(tabHandling._file);
                tabHandling._firstData = s;
                tabHandling._isFileSelected = true;
                tabHandling._isSaved = true;
            }

            TextEditor CodeBox =
                ((TextEditor) ((Grid) ((TabItem) FindName($"Tab{n}")).FindName(
                    "grid_codebox")).Children[0]);
            TextEditor PythonBox =
                ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[0]);
            
            // Events called in order to update attributes for research
            CodeBox.TextArea.GotFocus += CodeBoxSetLastElement;
            PythonBox.TextArea.GotFocus += PythonBoxSetLastElement;
            
            // Events called in order to manage the CTRL + Scroll with mouse
            CodeBox.TextArea.MouseWheel += OnMouseDownMain;
            PythonBox.TextArea.MouseWheel += OnMouseDownMain;
            
            // Events called on text typing for autocompletion
            CodeBox.TextArea.KeyDown += CodeBox_TextArea_TextEntering;
        }
        

        private MessageBoxResult messageOnClose(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Data App",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            return result;
        }

        private void IDL_Closing(object sender, RoutedEventArgs cancelEventArgs)
        {
            string paths = "";
            bool cancelled = false;
            foreach (TabItem item in ((TabablzControl) FindName("TabControl"))
                .Items)
            {
                _currenttabId = item.Name.Replace("Tab", "");
                if (!attributes.TryGetValue(_currenttabId,
                    out _currentTabHandler))
                {
                    Console.WriteLine(
                        "Warning in CurrenttabHandler (On selection changed)");
                }

                if (!_currentTabHandler._isFileSelected && DataChanged())
                {
                    MessageBoxResult result = messageOnClose(
                        "Your changes on the file: " + item.Header +
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
                            ((TextEditor) FindName("CodeBox" + _currenttabId))
                            .Text);
                        paths += Path.GetFullPath(_currentTabHandler._file) +
                                 '\n';
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

        // Function in order to unregister previous instances (used for closing a tab)
        private void UnregisterNamesAndRemove()
        {
            Console.WriteLine("Remove " + _currenttabId);
            UnregisterName("Tab" + _currenttabId);
            UnregisterName("CodeBox" + _currenttabId);
            UnregisterName("python" + _currenttabId);
            UnregisterName("STD" + _currenttabId);
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(
                _currentTab);
            TabControl.SelectedIndex =
                ((TabablzControl) FindName("TabControl")).Items.Count - 1;
        }

        private void CloseTab()
        {
            if (!_currentTabHandler._isFileSelected && DataChanged())
            {
                string msg = "Do you want to save your changes ?\n";
                MessageBoxResult result = MessageBox.Show(msg, "Data App",
                    MessageBoxButton.YesNoCancel,
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
                WriteAllTextSafe();
                UnregisterNamesAndRemove();
            }
        }
        
        private void TabControl_OnSelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (((TabablzControl) e.Source).SelectedIndex == -1)
            {
                ((TabablzControl) e.Source).SelectedIndex = 0;
            }

            Console.WriteLine("Added items " + e.AddedItems.Count +
                              ", Removed items " + e.RemovedItems.Count);
            if (e.AddedItems.Count != 0)
            {
                var source = (TabItem) e.AddedItems[0];
                _currentTab = ((TabablzControl) e.Source).SelectedIndex;
                _currenttabId = source.Name.Replace("Tab", "");
                Console.WriteLine("Current tab id " + _currenttabId);
                if (!attributes.TryGetValue(_currenttabId,
                    out _currentTabHandler))
                {
                    Console.WriteLine(
                        "Warning in CurrenttabHandler (On selection changed)");
                }
            }
            else
            {
                var source = (TabItem) e.RemovedItems[0];
                string i = source.Name.Replace("Tab", "");
                attributes.Remove(i);
            }
        }

        private void Set_Tab(int n)
        {
            TabControl.SelectedIndex = n - 1;
            if (!attributes.TryGetValue(_currenttabId, out _currentTabHandler))
            {
                Console.WriteLine(
                    "Warning in CurrenttabHandler (On selection changed)");
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(1);
            if (e.Key == Key.D2 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(2);
            if (e.Key == Key.D3 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(3);
            if (e.Key == Key.D4 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(4);
            if (e.Key == Key.D5 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(5);
            if (e.Key == Key.D6 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(6);
            if (e.Key == Key.D7 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(7);
            if (e.Key == Key.D8 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(8);
            if (e.Key == Key.D9 && Keyboard.Modifiers == ModifierKeys.Control)
                Set_Tab(9);
        }
    }
}