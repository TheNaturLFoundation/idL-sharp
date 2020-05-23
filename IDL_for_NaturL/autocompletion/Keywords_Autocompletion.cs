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
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using Dragablz;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using MaterialDesignThemes.Wpf;
using HighlightingManager =
    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        public List<string> ConstantKeywords = new List<string>()
        {
            "fonction",
            "variables",
            "debut",
            "pour",
            "de",
            "faire",
            "retourner",
            "fin",
            "si",
            "sinon_si",
            "tant_que",
            "alors",
            "procedure",
            "jusqu_a",
            "utiliser",
            "sinon",
            "et",
            "non",
            "entier",
            "reel",
            "booleen",
            "chaine",
            "caractere",
            "liste",
            "rien",
            "vrai",
            "faux",
        };

        public List<TextDocumentContentChangeEvent> documentsList = new List<TextDocumentContentChangeEvent>();
        public List<string> ContextKeywords = new List<string>();

        public int version;

        // This function will get the last typed word and update an attribute
        public void TemplatesManagerOnTab(TextEditor textEditor, KeyEventArgs e)
        {
            int i = textEditor.CaretOffset;
            int countBn = 0; // Count Back-slash n
            while (i < textEditor.Text.Length)
            {
                if (countBn == 4)
                    return;
                if (textEditor.Text[i] == '\n')
                    countBn++;
                if (textEditor.Text[i] == '\\')
                {
                    int length = 1;
                    int j = ++i;
                    while (j < textEditor.Text.Length && textEditor.Text[j] != '/')
                    {
                        length++;
                        j++;
                    }

                    e.Handled = true;
                    textEditor.Select(i - 1, length + 1);
                    return;
                }

                i++;
            }
        }

        public string GetLastTypedWord(string lastTypedWord, TextEditor textEditor)
        {
            List<char> specialChars = new List<char>() {' ', '\n', '\t', '\r', '(', ')'};
            int offset = textEditor.CaretOffset - 1;
            while (offset > -1 && !specialChars.Contains(textEditor.Text[offset]))
            {
                char appendChar = textEditor.Text[offset];
                lastTypedWord = (appendChar == '\x00' ? "" : appendChar.ToString()) + lastTypedWord;
                offset--;
            }

            return lastTypedWord;
        }

        public IList<ICompletionData> GetDataList(string lastTypedWord, TextEditor textEditor)
        {
            completionWindow =
                CompletionWindow.GetInstance(textEditor.TextArea);
            IList<ICompletionData> data =
                completionWindow.CompletionList.CompletionData;
            TextLocation textLocation = textEditor.Document.GetLocation(
                textEditor.CaretOffset);
            Position position = new Position(textLocation.Line, textLocation.Column);
            string file = _currentTabHandler._file;
            string path = string.IsNullOrEmpty(file) ? _currentTabHandler.playground : file;
            LspSender.RequestKeywords(position, path);
            Dictionary<string,float> keywordsScore = new Dictionary<string, float>(); 
            ContextKeywords.ForEach(keyword => keywordsScore.Add(keyword,CompletionScore(keyword,lastTypedWord)));
            IOrderedEnumerable<string> sorted =
                ContextKeywords.Where(keyword => keywordsScore[keyword] >= 0)
                    .OrderBy(keyword  => keywordsScore[keyword]);

            foreach (var keyword in sorted)
            {
                MyCompletionData myCompletionData =
                    new MyCompletionData(keyword, language, textEditor);
                data.Add(myCompletionData);
            }

            return data;
        }

        public void AutoComplete(TextEditor textEditor, KeyEventArgs e)
        {
            string lastTypedWord = KeyUtil.KeyToChar(e.Key).ToString();
            if (textEditor.CaretOffset > 0)
            {
                lastTypedWord = GetLastTypedWord(lastTypedWord == '\x00'.ToString() ? "" : lastTypedWord, textEditor);
            }

            IList<ICompletionData> data = GetDataList(lastTypedWord, textEditor);
            if (data.Count == 0)
            {
                completionWindow.Close();
            }
            if (data.Count != 0 && (e.Key != Key.Back || !string.IsNullOrEmpty(lastTypedWord)))
            {
                completionWindow.Show();
            }
            completionWindow.CompletionList.SelectItem("");
        }

        public Dictionary<T,Q> ReverseDictionary<T,Q>(Dictionary<Q,T> dictionary)
        {
            return dictionary.ToDictionary(element => element.Value, element => element.Key);
        }
        public void AutoCompleteSpecialChars(char @char)
        {
            Dictionary<char, char> specialChars = 
                new Dictionary<char, char> {{'\'','\''},{'\"','\"'},{'\\','/'},{'[',']'},{'(',')'}};
            var reversed = ReverseDictionary(specialChars);
            if (!specialChars.TryGetValue(@char,out char char1)) return;
            _lastFocusedTextEditor.Document.Insert(_lastFocusedTextEditor.CaretOffset, char1.ToString());
            _lastFocusedTextEditor.CaretOffset -= 1;
            if (!reversed.TryGetValue(@char, out char char2)) return;
            _lastFocusedTextEditor.Document.Remove(_lastFocusedTextEditor.CaretOffset,2);
            _lastFocusedTextEditor.CaretOffset += 1;
        }
        public void CodeBox_TextArea_KeyDown(object sender, TextCompositionEventArgs e)
        {
            AutoCompleteSpecialChars(e.Text[0]);
        }
        public void CodeBox_TextArea_TextEntering(object sender, KeyEventArgs e)
        {
            string currentUri = _currentTabHandler._file ?? _currentTabHandler.playground;
            documentsList.Add(new TextDocumentContentChangeEvent(_lastFocusedTextEditor.Text));
            LspSender.DidChangeNotification(new VersionedTextDocumentIdentifier(++version, currentUri), documentsList);
            switch (e.Key)
            {
                case Key.Escape:
                    _lastFocusedTextEditor.Select(_lastFocusedTextEditor.CaretOffset, 0);
                    return;
                case Key.Tab:
                    TemplatesManagerOnTab(_lastFocusedTextEditor, e);
                    return;
            }

            char typed = KeyUtil.KeyToChar(e.Key);
            if ((e.Key == Key.Back && string.IsNullOrEmpty(typed.ToString())) || (!char.IsLetterOrDigit(typed) && e.Key != Key.Back))
            {
                completionWindow?.Close();
                return;
            }

            Dispatcher.InvokeAsync(() => AutoComplete(_lastFocusedTextEditor, e));
        }

        private float CompletionScore(string reference, string input)
        {
            float sum = 0;
            
            Dictionary<char, int> indexes = new Dictionary<char, int>();
            for (var index = 0; index < reference.Length; index++)
            {
                char chr = reference[index];
                try
                {
                    indexes.Add(chr, index);
                }
                catch (ArgumentException e)
                {
                }
            }

            for (var i = 0; i < input.Length; i++)
            {
                var chr = input[i];
                if (chr == '\x00') continue;
                if (!indexes.TryGetValue(chr, out int index))
                {
                    index = -1;
                }
                else
                {
                    indexes.Remove(chr);
                }
                index += 1;
                sum += index * 1.0f / input.Length;
                if (index == 0)
                {
                    return -1;
                }
            }
            return sum;
        }


        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text, Language language, TextEditor lastfocusedtexteditor)
            {
                this.Text = text;
                this.language = language;
                this.Lastfocusedtexteditor = lastfocusedtexteditor;
            }

            public System.Windows.Media.ImageSource Image
            {
                get { return null; }
            }

            public TextEditor Lastfocusedtexteditor { get; private set; }
            public Language language { get; private set; }
            public string Text { get; private set; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content
            {
                get { return this.Text; }
            }

            public object Description
            {
                get { return "Description for " + this.Text; }
            }
            //+ SetDesc(Text)

            public double Priority { get; }

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                int offset = Lastfocusedtexteditor.CaretOffset;
                while (offset > 0 && (char.IsLetterOrDigit(Lastfocusedtexteditor.Text[offset - 1])
                                      || Lastfocusedtexteditor.Text[offset - 1] == '_'))
                {
                    offset--;
                }

                MySegment mySegment;
                if (offset == 1)
                {
                    mySegment = new MySegment(0, 0, 0);
                    textArea.Document.Text = "";
                    Lastfocusedtexteditor.CaretOffset = 0;
                }
                else
                {
                    mySegment = new MySegment(offset,
                        completionSegment.EndOffset - offset
                        , completionSegment.EndOffset);
                    Lastfocusedtexteditor.CaretOffset = offset;
                }

                int copyoffset = offset;
                textArea.Document.Replace(mySegment, SetTextDep());
                Lastfocusedtexteditor.CaretOffset = SetOffSet(offset, Lastfocusedtexteditor.Text, SetTextDep().Length);
                ScrollIfIsLastsLines(copyoffset);
            }

            public string SetTextDep()
            {
                switch (Text)
                {
                    case "fonction":
                        return "fonction \\NOM/(\\PARAMETRES/) -> \\TYPE_RETOUR/\n" +
                               "\tvariables\n\t\t\\VARIABLES/\n" +
                               "debut\n\t\\CODE/\n\tretourner\nfin";
                    case "variables":
                        return "variables";
                    case "debut":
                        return "debut\nfin";
                    case "pour":
                        return "pour \\VAR/ de \\DEBUT/ jusqu_a \\FIN/\nfin";
                    case "de":
                        return "de";
                    case "jusqu_a":
                        return "jusqu_a";
                    case "faire":
                        return "faire";
                    case "retourner":
                        return "retourner";
                    case "fin":
                        return "fin\n";
                    case "si":
                        return "si \\CONDITION/ alors\n\t\\CODE/\nfin\n";
                    case "sinon_si":
                        return "sinon_si \\CONDITION/ alors\n\t\\CODE/\n";
                    case "tant_que":
                        return "tant_que \\CONDITION/ faire\n\t\\CODE/\nfin";
                    case "alors":
                        return "alors";
                    case "procedure":
                        return "procedure \\NOM/(\\PARAMETRES/)\n" +
                               "\tvariables\n\t\t\\VARIABLES/\n" +
                               "debut\n\t\\CODE/\nfin";
                    case "utiliser":
                        return "utiliser";
                    case "sinon":
                        return "sinon\n\t\\CODE/";
                    default:
                        return Text;
                }
            }

            public int SetOffSet(int offset, string text, int align)
            {
                int i = offset;
                while (i < text.Length)
                {
                    if (text[i] == '\\')
                    {
                        return offset;
                    }

                    if (text[i] == '\n')
                    {
                        return offset + align;
                    }

                    i++;
                }

                return i;
            }

            public int GetLineFromIndex(int index)
            {
                int line = 0;
                foreach (var chr in _lastFocusedTextEditor.Text)
                {
                    if (chr == '\n')
                        line++;
                    index--;
                    if (index == 0)
                        return line;
                }

                return line;
            }

            public int CountLines()
            {
                int count = 0;
                foreach (var chr in _lastFocusedTextEditor.Text)
                {
                    if (chr == '\n')
                    {
                        count++;
                    }
                }

                return count;
            }

            public void ScrollIfIsLastsLines(int offset)
            {
                if (GetLineFromIndex(offset) + 10 >= CountLines())
                {
                    _lastFocusedTextEditor.ScrollToEnd();
                }
            }
        }
    }
}

public class MySegment : ISegment
{
    public int Offset { get; }
    public int Length { get; }
    public int EndOffset { get; }

    public MySegment(int offset, int length, int endOffset)
    {
        Offset = offset;
        Length = length;
        EndOffset = endOffset;
    }
}