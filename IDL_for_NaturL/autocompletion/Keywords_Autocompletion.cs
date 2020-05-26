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
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
            "booleen",
            "caractere",
            "liste",
            "rien",
            "vrai",
            "faux",
        };

        private Queue<TextDocumentContentChangeEvent> documentsList = new Queue<TextDocumentContentChangeEvent>();
        private List<string> ContextKeywords = new List<string>();
        private Dictionary<string, string> keyWordsDescriptions = new Dictionary<string, string>();
        private double lastTypedTime;
        private int version;

        // This function will get the last typed word and update an attribute
        private void TemplatesManagerOnTab(TextEditor textEditor, KeyEventArgs e)
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

        private string GetLastTypedWord(string lastTypedWord, TextEditor textEditor)
        {
            List<char> specialChars = new List<char>() {' ', '\n', '\t', '\r', '(', ')'};
            int offset = textEditor.CaretOffset - 1;
            while (offset > -1 && !specialChars.Contains(textEditor.Text[offset]))
            {
                char appendChar = textEditor.Text[offset];
                lastTypedWord = appendChar + lastTypedWord;
                offset--;
            }

            return lastTypedWord;
        }

        private IList<ICompletionData> GetDataList(string lastTypedWord, TextEditor textEditor)
        {
            Console.WriteLine("Called Get Data list");
            completionWindow =
                CompletionWindow.GetInstance(textEditor.TextArea);
            IList<ICompletionData> data =
                completionWindow.CompletionList.CompletionData;
            Dictionary<string, float> keywordsScore = new Dictionary<string, float>();
            IOrderedEnumerable<string> sorted;
            ContextKeywords.ForEach(keyword =>
                keywordsScore.TryAdd(keyword, CompletionScore(keyword, lastTypedWord)));
            sorted = ContextKeywords.Where(keyword => { return keywordsScore[keyword] >= 0; })
                .OrderBy(keyword => keywordsScore[keyword]);


            foreach (var keyword in sorted)
            {
                if (keyword == null)
                {
                    continue;
                }

                string description = default;
                keyWordsDescriptions?.TryGetValue(keyword, out description);
                MyCompletionData myCompletionData =
                    new MyCompletionData(keyword, description, language, textEditor, () => CodeBoxText(null, null));
                data.Add(myCompletionData);
            }

            return data;
        }

        private void AutoComplete(TextEditor textEditor)
        {
            string lastTypedWord = "";
            if (textEditor.CaretOffset > 0)
            {
                lastTypedWord = GetLastTypedWord(lastTypedWord, textEditor);
            }

            IList<ICompletionData> data;
            lock (ContextKeywords)
            {
                data = GetDataList(lastTypedWord, textEditor);
            }
            if (data.Count == 0)
            {
                completionWindow.Close();
            }

            if (data.Count != 0 && !string.IsNullOrEmpty(lastTypedWord))
            {
                completionWindow.Show();
            }

            completionWindow.CompletionList.SelectItem("");
        }

        public Dictionary<T, Q> ReverseDictionary<T, Q>(Dictionary<Q, T> dictionary)
        {
            return dictionary.ToDictionary(element => element.Value, element => element.Key);
        }

        public void AutoCompleteSpecialChars(char @char)
        {
            TextEditor editor = _lastFocusedTextEditor;
            Dictionary<char, char> specialChars =
                new Dictionary<char, char>
                {
                    {'\'', '\''}, {'\"', '\"'},
                    {'\\', '/'}, {'[', ']'}, {'(', ')'}
                };
            if (specialChars.TryGetValue(@char, out char char1)
                && (!(editor.CaretOffset < editor.Text.Length) || char1 != editor.Text[editor.CaretOffset]))
            {
                editor.Document.Insert(editor.CaretOffset, char1.ToString());
                editor.CaretOffset -= 1;
            }
            else if (editor.CaretOffset < editor.Text.Length &&
                     specialChars.ContainsValue(@char) && @char == editor.Text[editor.CaretOffset])
            {
                editor.Document.Remove(editor.CaretOffset, 1);
            }
        }


        public void DeleteSpecialChars()
        {
            Dictionary<char, char> specialChars =
                new Dictionary<char, char>
                {
                    {'\'', '\''}, {'\"', '\"'},
                    {'\\', '/'}, {'[', ']'}, {'(', ')'}
                };
            TextEditor editor = _lastFocusedTextEditor;
            int offset = editor.CaretOffset;
            if (editor.CaretOffset > 0 && editor.CaretOffset < editor.Text.Length
                                       && specialChars.TryGetValue(editor.Text[offset - 1], out char ch1)
                                       && specialChars[editor.Text[offset - 1]] == editor.Text[offset])
            {
                editor.Document.Remove(offset, 1);
            }
        }

        public void CodeBox_TextArea_KeyDown(object sender, TextCompositionEventArgs e)
        {
            char got = e == null ? '\x08' : e.Text[0];
            AutoCompleteSpecialChars(got);
            if (got == '\x08')
            {
                DeleteSpecialChars();
            }

            if ((got == '\x08' && string.IsNullOrEmpty(got.ToString()))
                || (!char.IsLetterOrDigit(got) && got != '\x08'))
            {
                completionWindow?.Close();
                return;
            }

            TextEditor textEditor = _lastFocusedTextEditor;
            TextLocation textLocation = textEditor.Document.GetLocation(
                textEditor.CaretOffset);
            Position position = new Position(textLocation.Line - 1, textLocation.Column - 1);
            string file = _currentTabHandler._file;
            string path = string.IsNullOrEmpty(file) ? _currentTabHandler.playground : "file://" + file;
            LspSender.RequestKeywords(position, path);
            AutoComplete(_lastFocusedTextEditor);
        }

        public double GetTimeStamp()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public void SendChanges(string currentUri)
        {
            double currentTime = GetTimeStamp();
            if (currentTime - lastTypedTime > 400)
            {
                Dispatcher.Invoke(() =>
                {
                    LspSender.DidChangeNotification(
                        new VersionedTextDocumentIdentifier(++version, currentUri),
                        documentsList.Reverse());
                });
            }
        }

        public void CodeBoxText(object sender, TextCompositionEventArgs e)
        {
            string currentUri = _currentTabHandler._file == null
                ? _currentTabHandler.playground
                : "file://" + _currentTabHandler._file;
            string text = _lastFocusedTextEditor.Text;
            if (sender is Key key && key == Key.Back && _lastFocusedTextEditor.CaretOffset > 1)
            {
                text = text.Remove(_lastFocusedTextEditor.CaretOffset - 1, 1);
            }

            text = text.Replace("\r", "");
            documentsList.Enqueue(new TextDocumentContentChangeEvent(text));
            lock (this)
            {
                lastTypedTime = GetTimeStamp();
            }

            Thread thread = new Thread(_ =>
            {
                Thread.Sleep(500);
                SendChanges(currentUri);
            });
            thread.Start();
            if (documentsList.Count > 10)
            {
                documentsList.Dequeue();
            }
        }

        public void CodeBox_TextArea_TextEntering(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _lastFocusedTextEditor.Select(_lastFocusedTextEditor.CaretOffset, 0);
                    return;
                case Key.Tab:
                    TemplatesManagerOnTab(_lastFocusedTextEditor, e);
                    return;
                case Key.Back:
                    CodeBox_TextArea_KeyDown(null, null);
                    CodeBoxText(Key.Back, null);
                    break;
                case Key.Y when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.X when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.C when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.V when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.Z when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.Enter:
                    Thread thread = new Thread(o =>
                    {
                        Thread.Sleep(10);
                        Dispatcher.Invoke(() => CodeBoxText(null, null));
                    });
                    thread.Start();
                    break;
            }
        }

        private float CompletionScore(string reference, string input)
        {
            float sum = 0;

            Dictionary<char, List<int>> indexes = new Dictionary<char, List<int>>();
            for (var index = 0; index < reference.Length; index++)
            {
                char chr = reference[index];
                try
                {
                    try
                    {
                        indexes[chr].Add(index);
                    }
                    catch (KeyNotFoundException)
                    {
                        indexes.Add(chr, new List<int> {index});
                    }
                }
                catch (ArgumentException e)
                {
                }
            }

            for (var i = 0; i < input.Length; i++)
            {
                int index;
                var chr = input[i];
                if (chr == '\x00') continue;
                if (!indexes.TryGetValue(chr, out List<int> indexlist))
                {
                    index = -1;
                }
                else
                {
                    try
                    {
                        index = indexlist[0];
                        indexlist.RemoveAt(0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        indexes.Remove(chr);
                        index = -1;
                    }
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
            public MyCompletionData(string text, string description, Language language,
                TextEditor lastfocusedtexteditor, Action OnTab)
            {
                this.Text = text;
                this.description = description;
                this.language = language;
                this.Lastfocusedtexteditor = lastfocusedtexteditor;
                this.action = OnTab;
            }

            private Action action;

            public System.Windows.Media.ImageSource Image
            {
                get { return null; }
            }

            private string description;
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
                get { return this.description; }
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
                action();
            }

            public string SetTextDep()
            {
                switch (Text)
                {
                    case "fonction":
                        return "fonction \\NOM/(\\PARAMETRES/) -> \\TYPE_RETOUR/\n" +
                               "\tvariables\n\t\t\\VARIABLES/\n" +
                               "debut\n\t\\CODE/\nfin";
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