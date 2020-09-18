using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDL_for_NaturL.filemanager;


namespace IDL_for_NaturL
{
    public partial class MainWindow : LspReceiver
    {
        public LspSender LspSender;
        
        private Dictionary<string, Dictionary<int, (string, DiagnosticSeverity)>> uriMessages =
            new Dictionary<string, Dictionary<int, (string, DiagnosticSeverity)>>();

        public void JumpToDefinition(Location location)
        {
            string uri = location.uri;
            Range range = location.range;
            Position start = range.start;
            Position end = range.end;
            // implement logic for the uri in order to open file if not opened
            // and select the tab if not selected
            Dispatcher.Invoke(() => Open_Click(uri));
            int startPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(start.line + 1, start.character + 1));
            int endPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(end.line + 1, end.character + 1));
            Dispatcher.Invoke(() => _lastFocusedTextEditor.Select(startPos, endPos - startPos + 1));
            Dispatcher.Invoke(() => _lastFocusedTextEditor.ScrollToLine(start.line));
        }


        public void Completion(IList<CompletionItem> keywords)
        {
            lock (ContextKeywords)
            {
                ContextKeywords = new List<string>();
                keyWordsDescriptions = new Dictionary<string, string>();
                foreach (var keyword in keywords)
                {
                    keyWordsDescriptions.TryAdd(keyword.label, keyword.detail);
                }

                ContextKeywords.AddRange(keywords.Select(item => item.label));
                ContextKeywords.AddRange(ConstantKeywords);
            }
        }
        

        private string GetTabFromUri(string uri)
        {
            foreach (var (key, value) in tabAttributes)
            {
                string editorFile = value._file == null ? value.playground : "file://" + value._file;
                if (uri == editorFile)
                {
                    return key;
                }
            }

            throw new ArgumentOutOfRangeException(); // not supposed to be reached
        }

        private int DeleteWhiteSpaceLine(int line)
        {
            TextEditor textEditor = _lastFocusedTextEditor;
            if (line >= textEditor.Text.Length) return 0;
            string[] text = textEditor.Text.Split('\n');
            if (line >= text.Length)
            {
                return 0;
            }

            string linetext = text[line];
            int offset = 1;
            foreach (char @char in linetext)
            {
                if (!string.IsNullOrEmpty(linetext) && @char != ' ' && @char != '\t')
                {
                    return textEditor.Document.GetOffset(line + 1, offset);
                }

                offset++;
            }

            return textEditor.Document.GetOffset(line + 1, 0);
        }

        private void SetLineTransformers(Diagnostic[] diagnostics, string uri)
        {
            string tabindex = GetTabFromUri(uri);
            TextEditor actualEditor = _lastFocusedTextEditor;
            foreach (var diagnostic in diagnostics)
            {
                TextEditor editor = (TextEditor) FindName("CodeBox" + tabindex);
                DiagnosticSeverity severity = diagnostic.severity ?? DiagnosticSeverity.Information;
                int line = diagnostic.range.start.line;
                editor.TextArea.TextView.LineTransformers.Add(
                    new LineColorizer(line + 1, severity, 
                        DeleteWhiteSpaceLine(line)));
                lock(uriMessages)
                {
                    if (uriMessages.TryGetValue(uri, out var lineMessages))
                    {
                        uriMessages[uri][line + 1] = (diagnostic.message, severity);
                    }
                    else
                    {
                        uriMessages[uri] = new Dictionary<int, (string, DiagnosticSeverity)>()
                        {
                            {line + 1, (diagnostic.message, severity)}
                        };
                    }
                }
                UpdateLayout();
                _lastFocusedTextEditor = actualEditor;
            }
        }

        private void MouseCaptured(object sender, MouseButtonEventArgs e)
        {
            toolTip.IsOpen = false;
            TextLocation location = _lastFocusedTextEditor.Document.GetLocation(_lastFocusedTextEditor.CaretOffset);
            int line = location.Line;
            string uri = _currentTabHandler._file == null
                ? _currentTabHandler.playground
                : "file://" + _currentTabHandler._file;
            if (uriMessages.TryGetValue(uri, out var lineMessages))
            {
                if (lineMessages.TryGetValue(line, out (string, DiagnosticSeverity) tuple))
                {
                    string message = tuple.Item1;
                    DiagnosticSeverity warning = tuple.Item2;
                    var caret = _lastFocusedTextEditor.TextArea.Caret.CalculateCaretRectangle();
                    toolTip.HorizontalOffset = caret.Width;
                    toolTip.VerticalOffset = caret.Height;
                    toolTip.Content = message;
                    toolTip.Foreground = GetBrushColorFromSeverity(warning);
                    toolTip.FontWeight = FontWeights.Bold;
                    toolTip.IsOpen = true;
                }
            }
        }

        private Brush GetBrushColorFromSeverity(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Error:
                    return Brushes.Red;
                case DiagnosticSeverity.Warning:
                    return Brushes.Orange;
                case DiagnosticSeverity.Information:
                    return Brushes.Black;
                case DiagnosticSeverity.Hint:
                    break;
            }

            throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
        }

        public void Diagnostic(Diagnostic[] diagnostics, string uri)
        {
            Dispatcher.Invoke(() =>
            {
                TextEditor editor = (TextEditor) FindName("CodeBox" + GetTabFromUri(uri));
                if (uriMessages.ContainsKey(uri))
                {
                    uriMessages[uri].Clear();
                }
                editor.TextArea.TextView.LineTransformers.Clear();
                XmlDocument doc = new XmlDocument();
                doc.Load(UserSettings.syntaxFilePath);
                editor.SyntaxHighlighting = HighlightingLoader.Load(new XmlNodeReader(doc),
                    HighlightingManager.Instance);
                SetLineTransformers(diagnostics, uri);
            });
        }

        public void Reformat(Range range, string newText)
        {
            Dispatcher?.Invoke(() => _lastFocusedTextEditor.Document.Text = newText);
        }

        private void ReformatRequest()
        {
            string uri = _currentTabHandler._file == null
                ? _currentTabHandler.playground
                : "file://" + _currentTabHandler._file;
            LspSender.FormattingRequest(uri, 2, false);
        }
    }
}