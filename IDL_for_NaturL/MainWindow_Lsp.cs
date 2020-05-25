using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Dragablz;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using IDL_for_NaturL.filemanager;

namespace IDL_for_NaturL
{
    public partial class MainWindow : LspReceiver
    {
        public LspSender LspSender;
        
        public void JumpToDefinition(Location location)
        {
            Console.WriteLine(location.ToString());
            string uri = location.uri;
            Range range = location.range;
            Position start = range.start;
            Position end = range.end;
            // implement logic for the uri in order to open file if not opened
            // and select the tab if not selected
            Dispatcher.Invoke(() => Open_Click(uri));
            int startPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(start.line+1, start.character+1));
            int endPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(end.line+1, end.character+1));
            Dispatcher.Invoke(() => _lastFocusedTextEditor.Select(startPos, endPos - startPos));
        }

        

        public void Completion(IList<CompletionItem> keywords)
        {
            ContextKeywords = new List<string>();
            ContextKeywords.AddRange(keywords.Select(item => item.label));
            ContextKeywords.AddRange(ConstantKeywords);
        }

        public void ClearLineTransformers(string uri)
        {
            if (uri.Contains("file://"))
            {
                uri = uri.Replace("file://", "");
            }
            TextEditor actualEditor = _lastFocusedTextEditor;
            TextEditor textEditor;
            foreach (var (key,value) in tabAttributes)
            {
                textEditor = (TextEditor) FindName("CodeBox" + key);
                string editorFile = value._file ?? value.playground;
                if (editorFile == uri)
                {
                    textEditor.TextArea.TextView.LineTransformers.Clear();
                    _lastFocusedTextEditor = actualEditor;
                    return;
                }
            }
        }

        public string GetTabFromUri(string uri)
        {
            if (uri.Contains("file://"))
            {
                uri = uri.Replace("file://", "");
            }
            foreach (var (key,value) in tabAttributes)
            {
                string editorFile = value._file ?? value.playground;
                if (uri == editorFile)
                {
                    return key;
                }
            }
            throw new ArgumentOutOfRangeException(); // not supposed to be reached
        }
        public void SetLineTransformers(Diagnostic[] diagnostics, string uri)
        {
            string tabindex = GetTabFromUri(uri);
            TextEditor actualEditor = _lastFocusedTextEditor;
            foreach (var diagnostic in diagnostics)
            {
                TextEditor editor = (TextEditor) FindName("CodeBox" + tabindex);
                DiagnosticSeverity severity = diagnostic.severity ?? DiagnosticSeverity.Information;
                int line = diagnostic.range.start.line;
                editor.TextArea.TextView.LineTransformers.Add(new LineColorizer(line+1,severity));
                Console.WriteLine("Severity is : " + severity);
                Console.WriteLine(line+1);
                UpdateLayout();
                _lastFocusedTextEditor = actualEditor;
            }
        }

        public void Diagnostic(Diagnostic[] diagnostics, string uri)
        {
            Dispatcher.InvokeAsync(() =>
            {
                ClearLineTransformers(uri);
                TextEditor editor = (TextEditor) FindName("CodeBox" + GetTabFromUri(uri));
                XmlDocument doc = new XmlDocument();
                doc.Load("resources/user_coloration.xshd");
                editor.SyntaxHighlighting = HighlightingLoader.Load(new XmlNodeReader(doc),
                    HighlightingManager.Instance);
                SetLineTransformers(diagnostics, uri);
            });
        }

        private void JumpToDefinitionEvent(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            TextLocation location = _lastFocusedTextEditor.Document.GetLocation(
                _lastFocusedTextEditor.CaretOffset);
            string filename = _currentTabHandler._file;
            string path = string.IsNullOrEmpty(filename) ? _currentTabHandler.playground : "file://" + filename;
            Dispatcher.Invoke(() => 
                LspSender.RequestDefinition(new Position(location.Line-1,location.Column), path));
        }
        
    }
}