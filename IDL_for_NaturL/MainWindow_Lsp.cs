using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Dragablz;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

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
                _lastFocusedTextEditor.Document.GetOffset(start.line + 1, start.character + 1));
            int endPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(end.line + 1, end.character + 1));
            Dispatcher.Invoke(() => _lastFocusedTextEditor.Select(startPos, endPos - startPos));
        }

        

        public void Completion(IList<CompletionItem> keywords)
        {
            ContextKeywords = new List<string>();
            ContextKeywords.AddRange(keywords.Select(item => item.label));
            ContextKeywords.AddRange(ConstantKeywords);
        }

        public void Diagnostic(Range range, DiagnosticSeverity warningSeverity, string message, string uri)
        {
            throw new NotImplementedException();
        }

        private void JumpToDefinitionEvent(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            TextLocation location = _lastFocusedTextEditor.Document.GetLocation(
                _lastFocusedTextEditor.CaretOffset);
            string filename = _currentTabHandler._file;
            string path = string.IsNullOrEmpty(filename) ? _currentTabHandler.playground : filename;
            Dispatcher.Invoke(() => 
                LspSender.RequestDefinition(new Position(location.Line,location.Column), path));
        }
    }
}