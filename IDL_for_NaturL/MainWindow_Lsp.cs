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
        [STAThread]
        public void JumpToDefinition(Location location)
        {
            string uri = location.uri;
            Range range = location.range;
            Position start = range.start;
            Position end = range.end;
            // implement logic for the uri in order to open file if not opened
            // and select the tab if not selected
            uri = uri.Replace("file://", "");
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

        public Position GetPositionFromIndex(int selectionStart)
        {
            int line = 0;
            int chrcount = 0;
            int indexcounter = 0;
            foreach (var chr in Dispatcher.Invoke(() => _lastFocusedTextEditor.Text))
            {
                if (chr == '\n')
                {
                    line++;
                    chrcount = 0;
                }

                if (indexcounter == selectionStart)
                {
                    return new Position(line, chrcount);
                }

                chrcount++;
                indexcounter++;
            }

            throw new ArgumentOutOfRangeException();
        }
        
        private void JumpToCommand_Executed(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Detected Click");
            if (e.ClickCount == 1 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Console.WriteLine("Detected Jump");
                TextLocation location = _lastFocusedTextEditor.Document.GetLocation(
                    _lastFocusedTextEditor.CaretOffset);
                LspSender.RequestDefinition(new Position(location.Line,location.Column), 
                    "file://" + Path.GetFullPath(_currentTabHandler._file));
            }
        }
    }
}