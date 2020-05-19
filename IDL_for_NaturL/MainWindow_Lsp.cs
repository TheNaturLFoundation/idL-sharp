using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace IDL_for_NaturL
{
    public partial class MainWindow : Lsp_Receiver
    {
        public LspSender LspSender;


        public void ReceiveKeywords(List<string> keywords)
        {
            throw new System.NotImplementedException();
        }

        public void JumpToDefinition(Location location)
        {
            Range range = location.range;
            Position start = range.start;
            Position end = range.end;
            int startPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(new TextLocation(start.line + 1, start.character + 1)));
            int endPos = Dispatcher.Invoke(() =>
                _lastFocusedTextEditor.Document.GetOffset(new TextLocation(end.line + 1, end.character + 1)));
            Dispatcher.Invoke(() => _lastFocusedTextEditor.Select(startPos, endPos - startPos));
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
                LspSender.RequestDefinition(GetPositionFromIndex(Dispatcher.Invoke(() =>
                        _lastFocusedTextEditor.SelectionStart)),
                    _currentTabHandler._file);
            }
        }
    }
}