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
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Data;
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
        public string[] Keywords =
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


        // This function will get the last typed word and update an attribute
        public void CodeBox_TextArea_TextEntering(object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                int i = _lastFocusedTextEditor.CaretOffset;
                int countBn = 0; // Count Back-slash n
                while (i < _lastFocusedTextEditor.Text.Length)
                {
                    if (countBn == 4)
                    {
                        return;
                    }

                    if (_lastFocusedTextEditor.Text[i] == '\n')
                    {
                        countBn++;
                    }

                    if (_lastFocusedTextEditor.Text[i] == '\\')
                    {
                        int length = 1;
                        int j = ++i;
                        while (j < _lastFocusedTextEditor.Text.Length && _lastFocusedTextEditor.Text[j] != '/')
                        {
                            length++;
                            j++;
                        }

                        e.Handled = true;
                        _lastFocusedTextEditor.Select(i - 1, length + 1);
                        return;
                    }

                    i++;
                }
            }

            if (!char.IsLetterOrDigit(KeyUtil.KeyToChar(e.Key)) && e.Key != Key.Back && e.Key != Key.Delete)
            {
                completionWindow?.Close();
                return;
            }

            string lastTypedWord = KeyUtil.KeyToChar(e.Key).ToString();
            if (_lastFocusedTextEditor.CaretOffset > 0)
            {
                int offset = _lastFocusedTextEditor.CaretOffset - 1;
                while (offset > -1 && _lastFocusedTextEditor.Text[offset] != ' '
                                   && _lastFocusedTextEditor.Text[offset] != '\n'
                                   && _lastFocusedTextEditor.Text[offset] != '\t'
                                   && _lastFocusedTextEditor.Text[offset] != '\r'
                                   && _lastFocusedTextEditor.Text[offset] != '('
                                   && _lastFocusedTextEditor.Text[offset] != ')')
                {
                    lastTypedWord = _lastFocusedTextEditor.Text[offset] + lastTypedWord;
                    offset--;
                }
            }

            completionWindow = CompletionWindow.GetInstance(_lastFocusedTextEditor.TextArea);
            IList<ICompletionData> data =
                completionWindow.CompletionList.CompletionData;
            // filter for strict completion Where(keyword => CompletionScore(keyword,lastTypedWord) > 0).
            var sorted = Keywords.Where(keyword => CompletionScore(keyword, lastTypedWord) >= 0)
                .OrderBy
                    (keyword => CompletionScore(keyword, lastTypedWord));
            foreach (var keyword in sorted)
            {
                MyCompletionData myCompletionData = new MyCompletionData(keyword, language, _lastFocusedTextEditor);
                data.Add(myCompletionData);
            }

            completionWindow.Show();
        }

        private float CompletionScore(string reference, string input)
        {
            float sum = 0;
            foreach (var chr in input)
            {
                int index = reference.IndexOf(chr) + 1;
                sum += index * 1.0f / (input.Length * 0.5f);
                if (index == 0)
                {
                    return -1;
                }
            }

            return sum;
        }


        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text, string language, TextEditor lastfocusedtexteditor)
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
            public string language { get; private set; }
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

                textArea.Document.Replace(mySegment, SetTextDep());
                Lastfocusedtexteditor.CaretOffset = SetOffSet(offset, Lastfocusedtexteditor.Text, SetTextDep().Length);
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
