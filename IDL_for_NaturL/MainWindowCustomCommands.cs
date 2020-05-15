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
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Dragablz;
using HighlightingManager = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace IDL_for_NaturL
{
    public static class CustomCommands
        {
            public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );

            //Define more commands here, just like the one above
            public static readonly RoutedUICommand Save = new RoutedUICommand
            (
                "Save",
                "Save",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand Open = new RoutedUICommand
            (
                "Open",
                "Open",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand NewTab = new RoutedUICommand
            (
                "NewTab",
                "NewTab",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.T, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand SaveAs = new RoutedUICommand
            (
                "Save_As",
                "Save_As",
                typeof(CustomCommands)
            );

            public static readonly RoutedUICommand Transpile = new RoutedUICommand
            (
                "Transpile",
                "Transpile",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F5, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand Execute = new RoutedUICommand
            (
                "Execute",
                "Execute",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F5)
                }
            );

            public static readonly RoutedUICommand New = new RoutedUICommand
            (
                "New",
                "New",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand CloseTab = new RoutedUICommand
            (
                "CloseTab",
                "CloseTab",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.W, ModifierKeys.Control)
                }
            );

            public static readonly RoutedUICommand Settings = new RoutedUICommand
            (
                "Settings",
                "Settings",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Alt)
                }
            );

            public static readonly RoutedUICommand Debug = new RoutedUICommand
            (
                "Debug",
                "Debug",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D, ModifierKeys.Control)
                }
            );
            public static readonly RoutedUICommand Research = new RoutedUICommand
            (
                "Research",
                "Research",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F, ModifierKeys.Control)
                }
            );
        }
}