using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Dragablz;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using HighlightingManager =
    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        private int _copyStart;

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
            if (!ResearchBox.IsFocused &&
                string.IsNullOrEmpty(ResearchBox.Text))
                ResearchBox.Text = "Search (Ctrl + F)";
        }


        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchInput(ResearchBox.Text);
        }

        public void SearchInput(string searched)
        {
            TextEditor currentEditor =
                (TextEditor) FindName("CodeBox" + _currenttabId);
            string fulltext = currentEditor.Text;
            int textlength = fulltext.Length;
            _copyStart = currentEditor.CaretOffset;
            if (_copyStart == textlength)
                _copyStart = 0;
            int index = fulltext.IndexOf(searched, _copyStart,
                textlength - _copyStart, StringComparison.Ordinal);
            if (index == -1)
            {
                MessageBoxResult messageBox = MessageBox.Show(
                    "No more occurences of \"" + searched + "\" found",
                    "Research", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                if (messageBox == MessageBoxResult.OK)
                {
                    currentEditor.Select(0, 0);
                    _copyStart = 0;
                    return;
                }
            }

            currentEditor.Select(index, searched.Length);
            Console.WriteLine("Caret Offset is: " + currentEditor.CaretOffset);
            if (GetLineFromIndex(index) > currentEditor.LineCount - 5)
                currentEditor.ScrollToEnd();
            else
                currentEditor.ScrollToVerticalOffset(
                    currentEditor.CaretOffset - 250);
            index += searched.Length;
            _copyStart = index;
            Console.WriteLine("Index is: " + index + " Length is: " +
                              fulltext.Length);
        }
    }
}