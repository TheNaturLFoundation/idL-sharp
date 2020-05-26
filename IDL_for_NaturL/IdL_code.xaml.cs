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
using System.Diagnostics;
using System.Windows.Media;
using Dragablz;
using ICSharpCode.AvalonEdit.CodeCompletion;
using IDL_for_NaturL.filemanager;
using HighlightingManager =
    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace IDL_for_NaturL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LspReceiver
    {
        private int _tabInt;
        private int _currentTab;
        private string _currenttabId = "0";
        private string tabitem;
        private int playgroundCount;
        private WarningSeverity _warningSeverity;

        private static Dictionary<string, TabHandling> tabAttributes =
            new Dictionary<string, TabHandling>();

        CompletionWindow completionWindow;
        private TabHandling _currentTabHandler;
        private IHighlightingDefinition _highlightingDefinition;
        private double clickPosition;
        public static MainWindow Instance { get; set; }

        private class TabHandling
        {
            public TabHandling(string file, int playgroundCount, bool isSaved = false)
            {
                _isFileSelected = !string.IsNullOrEmpty(file);
                _firstData = file == null ? "" : File.ReadAllText(file);
                _isSaved = isSaved;
                _file = file;
                this.playgroundCount = playgroundCount;
                playground = file == null ? "playground://playground" + playgroundCount : null;
            }


            //public TabHandling()
            public bool _isFileSelected { get; set; }
            public string _firstData { get; set; }
            public bool _isSaved { get; set; }
            public string _file { get; set; }
            public string playground { get; set; }
            public int playgroundCount { get; set; }

            public override string ToString() =>
                $"(Is File Selected : {_isFileSelected}, FirstData : {"first_data"}, IsSaved : {_isSaved}, File : {_file})";
        }

        //MyCompletionWindow myCompletionWindow;
        public MainWindow()
        {
            Instance = this;
            Environment.SetEnvironmentVariable("NATURLPATH", Path.GetFullPath("resources"));
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            //Loading Settings configuration from XML 
            UserSettings.LoadUserSettings("resources/settings.xml");
            language = UserSettings.language;
            _warningSeverity = UserSettings.warningSeverity;
            //Unused warning severity yet.
            InitializeComponent();
            TextEditor textEditor =
                (TextEditor) ((Grid) ((TabItem) FindName("Tab_id_")).FindName(
                    "grid_codebox")).Children[0];
            XmlTextReader reader = new XmlTextReader(UserSettings.syntaxFilePath);

            _highlightingDefinition =
                HighlightingLoader.Load(reader, HighlightingManager.Instance);

            textEditor.SyntaxHighlighting = _highlightingDefinition;
            reader.Close();
            string[] paths =
                File.ReadAllLines("resources/lastfiles.txt");
            tabitem = XamlWriter.Save(this.FindName("Tab_id_"));
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(0);
            Process processServer = new Process
            {
                StartInfo =
                {
                    FileName = "resources/server.exe",
                    EnvironmentVariables =
                    {
                        ["NATURLPATH"] =
                            Path.GetFullPath("resources")
                    },
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    ErrorDialog = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
            LspSender = new LspHandler(this, processServer);
            LspSender.InitializeRequest(Process.GetCurrentProcess().Id,
                "file://" + Directory.GetCurrentDirectory(),
                new ClientCapabilities(
                    new TextDocumentClientCapabilities(
                        new CompletionClientCapabilities(),
                        new DefinitionClientCapabilities(),
                        new PublishDiagnosticsClientCapabilities())));
            if (paths.Length == 0)
            {
                NewTabItems(_tabInt++, null);
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
            InitialiseLanguageComponents(language);
        }


        public void RemoveTab(int tabindex)
        {
            ((TabablzControl) FindName("TabControl")).Items.RemoveAt(tabindex);
        }

        [STAThread]
        private void NewTabItems(int n, string path)
        {
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
            TabHandling tabHandling = new TabHandling(path, path == null ? ++playgroundCount : playgroundCount);
            tabAttributes.Add(n.ToString(), tabHandling);
            ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "grid_codebox")).Children[0])
                .SyntaxHighlighting = _highlightingDefinition;
            ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[0])
                .SyntaxHighlighting = _highlightingDefinition;


            TabControl.SelectedIndex =
                ((TabablzControl) FindName("TabControl")).Items.Count - 1;
            if (!tabAttributes.TryGetValue(_currenttabId, out _currentTabHandler))
            {
                return;
            }

            if (path != null)
            {
                Dispatcher.Invoke(() => 
                    LspSender.DidOpenNotification(("file://" + path), "", 0, File.ReadAllText(path)));
                string s = File.ReadAllText(path);
                ((TextEditor) FindName("CodeBox" + n)).Text = s;
                ((TabItem) FindName("Tab" + n)).Header =
                    Path.GetFileNameWithoutExtension(tabHandling._file);
                tabHandling._firstData = s;
                tabHandling._isFileSelected = true;
                tabHandling._isSaved = true;
            }
            else
            {
                Dispatcher.Invoke(() => 
                    LspSender.DidOpenNotification(_currentTabHandler.playground, "", 
                        0, _currentTabHandler.playground));
                ((TabItem) FindName("Tab" + n)).Header =
                    Path.GetFileNameWithoutExtension(tabHandling.playground);
            }

            TextEditor CodeBox =
                ((TextEditor) ((Grid) ((TabItem) FindName($"Tab{n}")).FindName(
                    "grid_codebox")).Children[0]);
            TextEditor PythonBox =
                ((TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                    "python_grid")).Children[0]);
            TextEditor STD = 
                (TextEditor) ((Grid) ((TabItem) FindName("Tab" + n)).FindName(
                "python_grid")).Children[3];
            // Events called in order to update attributes for research
            CodeBox.TextArea.GotFocus += CodeBoxSetLastElement;
            PythonBox.TextArea.GotFocus += PythonBoxSetLastElement;

            // Events called in order to manage the CTRL + Scroll with mouse
            CodeBox.TextArea.MouseWheel += OnMouseDownMain;
            PythonBox.TextArea.MouseWheel += OnMouseDownMain;
            // Events called on text typing for autocompletion
            CodeBox.TextArea.PreviewKeyDown += CodeBox_TextArea_TextEntering;
            CodeBox.TextArea.TextEntered += CodeBox_TextArea_KeyDown;
            CodeBox.TextArea.TextEntered += CodeBoxText;
            // Events called on click
            BrushConverter converter = new BrushConverter();
            Brush brush = (Brush) converter.ConvertFrom("#ffeac8");
            Brush borderbrush = (Brush) converter.ConvertFrom("#ed9200");
            Pen pen = new Pen(borderbrush,0);
            CodeBox.TextArea.SelectionBorder = pen;
            CodeBox.TextArea.SelectionBrush = brush;
            PythonBox.TextArea.SelectionBorder = pen;
            PythonBox.TextArea.SelectionBrush = brush;
            STD.TextArea.SelectionBorder = pen;
            STD.TextArea.SelectionBrush = brush;
        }

        public void UpdateColorScheme(XmlDocument doc)
        {
            foreach (var element in tabAttributes)
            {
                TextEditor CodeBox =
                    ((TextEditor) ((Grid) ((TabItem) FindName($"Tab{element.Key}")).FindName(
                        "grid_codebox")).Children[0]);
                TextEditor PythonBox = ((TextEditor) ((Grid) ((TabItem) FindName($"Tab{element.Key}")).FindName(
                    "python_grid")).Children[0]);
                CodeBox.SyntaxHighlighting = HighlightingLoader.Load(new XmlNodeReader(doc),
                    HighlightingManager.Instance);
                PythonBox.SyntaxHighlighting = HighlightingLoader.Load(new XmlNodeReader(doc),
                    HighlightingManager.Instance);
                
            }
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
                if (!tabAttributes.TryGetValue(_currenttabId,
                    out _currentTabHandler))
                {
                    return;
                }

                if (!_currentTabHandler._isFileSelected && DataChanged())
                {
                    MessageBoxResult result = messageOnClose(
                        "Your changes on the file: " + item.Header +
                        " are not saved. \n Would you like to save them?");
                    if (result == MessageBoxResult.Yes)
                    {
                        bool saved = Save_Click();
                        if (!saved)
                        {
                            cancelled = true;
                        }
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

            File.WriteAllText("resources/lastfiles.txt",
                paths);
            if (!cancelled)
            {
                //Dispatcher.Invoke(() => LspSender.ExitNotification());
                Application.Current.Shutdown();
            }

            UserSettings.SaveUserSettings("resources/settings.xml");
        }

        // Function in order to unregister previous instances (used for closing a tab)
        private void UnregisterNamesAndRemove(bool saved = true)
        {
            if (saved)
            {
                UnregisterName("Tab" + _currenttabId);
                UnregisterName("CodeBox" + _currenttabId);
                UnregisterName("python" + _currenttabId);
                UnregisterName("STD" + _currenttabId);
                ((TabablzControl) FindName("TabControl")).Items.RemoveAt(
                    _currentTab);
                TabControl.SelectedIndex =
                    ((TabablzControl) FindName("TabControl")).Items.Count - 1;
                LspSender.DidCloseNotification(_currentTabHandler._file ?? _currentTabHandler.playground);
            }
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
                    bool saved = Save_Click();
                    UnregisterNamesAndRemove(saved);
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

            if (e.AddedItems.Count != 0)
            {
                var source = (TabItem) e.AddedItems[0];
                _currentTab = ((TabablzControl) e.Source).SelectedIndex;
                _currenttabId = source.Name.Replace("Tab", "");
                if (!tabAttributes.TryGetValue(_currenttabId,
                    out _currentTabHandler))
                {
                    return;
                }
            }
            else
            {
                var source = (TabItem) e.RemovedItems[0];
                string i = source.Name.Replace("Tab", "");
                tabAttributes.Remove(i);
            }
        }

        private void Set_Tab(int n)
        {
            TabControl.SelectedIndex = n - 1;
            if (!tabAttributes.TryGetValue(_currenttabId, out _currentTabHandler))
            {
                return;
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