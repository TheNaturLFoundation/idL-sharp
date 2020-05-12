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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Dragablz;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
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
            "jusqu_a",
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
            "and",
            "non"
        };

        public string LastTypedWord = "";

        public static Boolean isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        // This function will get the last typed word and update an attribute
        public void CodeBox_TextArea_TextEntering(object sender,
            KeyEventArgs e)
        {
            string fulltext = _lastFocusedTextEditor.Text;
            int index = _lastFocusedTextEditor.SelectionStart;
            fulltext =
                fulltext.Insert(index, KeyUtil.KeyToChar(e.Key).ToString());
            LastTypedWord = "";
            while (index > 0 && fulltext[index] != ' ' &&
                   fulltext[index] != '\n')
            {
                LastTypedWord = fulltext[index] + LastTypedWord;
                index--;
            }

            CompletionWindow completionWindow;
            completionWindow =
                new CompletionWindow(_lastFocusedTextEditor.TextArea);
            IList<ICompletionData> data =
                completionWindow.CompletionList.CompletionData;
            var sorted = Keywords.OrderBy
            (keyword =>
                LevenshteinUtils.Levenshtein(keyword, LastTypedWord));
            foreach (var word in sorted)
            {
                MyCompletionData myCompletionData = new MyCompletionData(word);
                data.Add(myCompletionData);
            }
            completionWindow.Show();
        }

        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text)
            {
                this.Text = text;
            }

            public System.Windows.Media.ImageSource Image
            {
                get { return null; }
            }
            
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

            public double Priority { get; }

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
            
        }
    }
}