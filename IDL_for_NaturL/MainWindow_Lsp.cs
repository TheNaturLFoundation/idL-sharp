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
            ContextKeywords.Clear();
            keyWordsDescriptions.Clear();
            foreach (var keyword in keywords)
            {
                keyWordsDescriptions.Add(keyword.label, keyword.detail);
            }
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

        public int DeleteWhiteSpaceLine(int line)
        {
            TextEditor textEditor = _lastFocusedTextEditor;
            string[] text = textEditor.Text.Split('\n');
            string linetext = text[line];
            int offset = 1;
            foreach (char @char in linetext)
            {
                if (!string.IsNullOrEmpty(linetext) && @char != ' ' && @char != '\t')
                {
                    return textEditor.Document.GetOffset(line+1,offset);
                }
                offset++;
            }

            return textEditor.Document.GetOffset(line+1,0);
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
                editor.TextArea.TextView.LineTransformers.Add(
                    new LineColorizer(line+1,severity,DeleteWhiteSpaceLine(line)));
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
                doc.Load(UserSettings.syntaxFilePath);
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