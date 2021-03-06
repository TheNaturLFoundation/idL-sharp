﻿using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        private int _copyStart;
        private int _occurrences;
        public static TextEditor _lastFocusedTextEditor;
        
        public int GetLineFromIndex(int index)
        {
            TextEditor currentEditor =
                (TextEditor) FindName("CodeBox" + _currenttabId);
            int line = 0;
            foreach (var chr in currentEditor.Text)
            {
                if (chr == '\n')
                    line++;
                index--;
                if (index == 0)
                    return line;
            }

            return line;
        }
        
        public void ResearchBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (ResearchBox.IsFocused)
                ResearchBox.Text = "";
        }

        public void ResearchBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!ResearchBox.IsFocused)
            {
                ResearchBox.Text = "Search (Ctrl + F)";
                Occurences.Text = "";
            }
        }
        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(ResearchBox.Text))
            {
                
                _occurrences = CountOccurences(_lastFocusedTextEditor.Text, ResearchBox.Text);
                Occurences.Text = _occurrences + " occurrence" + (_occurrences > 1 ? "s" : "");
                SearchInput(ResearchBox.Text, _lastFocusedTextEditor);
            }
        }

        public void SearchInput(string searched, TextEditor textEditor)
        {
            string fulltext = textEditor.Text;
            int textlength = fulltext.Length;
            _copyStart = textEditor.CaretOffset;
            if (_copyStart == textlength)
                _copyStart = 0;
            int index = fulltext.IndexOf(searched, _copyStart,
                textlength - _copyStart, StringComparison.Ordinal);
            if (index == -1)
            {
                string message = language == IDL_for_NaturL.Language.English
                    ? "No more occurences of \"" + searched + "\" found"
                    : "Plus d'occurences \"" + searched + "\" trouvées";
                MessageBoxResult messageBox = MessageBox.Show(message,
                    language == IDL_for_NaturL.Language.French ? "Rechercher" : "Research", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                if (messageBox == MessageBoxResult.OK)
                {
                    textEditor.Select(0, 0);
                    _copyStart = 0;
                    return;
                }
            }
            
            textEditor.Select(index, searched.Length);
            textEditor.ScrollToLine(textEditor.Document.GetLineByOffset(index).LineNumber);
            index += searched.Length;
            _copyStart = index;
        }

        private int CountOccurences(string fulltext, string input)
        {
            int textlength = fulltext.Length;
            int count = 0;
            int index = fulltext.IndexOf(input, 0,
                textlength, StringComparison.Ordinal);
            while (index != -1)
            {
                count++;
                index = fulltext.IndexOf(input, index + input.Length,
                    textlength - index - input.Length, StringComparison.Ordinal);
            }

            return count;
        }


        private void CodeBoxSetLastElement(object sender, RoutedEventArgs e)
        {
            _lastFocusedTextEditor.Select(0,0);
            _lastFocusedTextEditor =
                (TextEditor) FindName("CodeBox" + _currenttabId);
                        
        }
        private void PythonBoxSetLastElement(object sender, RoutedEventArgs e)
        {
            _lastFocusedTextEditor.Select(0,0);
            _lastFocusedTextEditor =
                (TextEditor) FindName("python" + _currenttabId);
        }
    }
}