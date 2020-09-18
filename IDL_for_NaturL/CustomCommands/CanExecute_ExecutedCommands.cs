using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Dragablz;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using IDL_for_NaturL.filemanager;
using MaterialDesignThemes.Wpf;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        private double lastTabTime;
        private double lastClosed;
        #region ExitCommand

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region SaveCommand

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_Click();
        }

        #endregion

        #region OpenCommand

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Click();
        }

        #endregion

        #region SaveAsCommand

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_AsClick();
        }

        #endregion

        #region ReformatCommand

        private void ReformatCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ReformatCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReformatRequest();
        }

        #endregion

        #region Transpile and Execute

        private void TranspileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TranspileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Transpile(sender, e);
        }

        private void ExecuteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !_processPythonRunning;
        }

        private void ExecuteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.InvokeAsync(() => Execute(sender, e));
        }

        #endregion

        #region NewTab

        private void NewTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            double currentTime = GetTimeStamp();
            e.CanExecute = currentTime - lastTabTime > 100;
        }

        private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewTabItems(++_tabInt, null);
            lastTabTime = GetTimeStamp();
        }

        #endregion

        #region NewWindow

        private void NewWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewWindow(sender, e);
        }

        #endregion

        #region New File

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewFile(sender, e);
        }

        #endregion

        private void CloseTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((TabablzControl) FindName("TabControl")).Items.Count > 1;
        }

        private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseTab();
        }

        private void SettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void DebugCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DebugCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TextEditor ed = _lastFocusedTextEditor;
            TextLocation location = ed.Document.GetLocation(ed.CaretOffset);
            LspSender.RequestDefinition(new Position(location.Line - 1, location.Column - 1),
                _currentTabHandler._file == null
                    ? _currentTabHandler.playground
                    : "file://" + _currentTabHandler._file);
        }

        private void ResearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ResearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResearchBox.Focus();
        }

        private void Cancel_ProcessCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _processPythonRunning;
        }

        private void Cancel_ProcessCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel_Process(null, null);
        }
        private void ResetZoomCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ResetZoomCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetZoomLevel();
        }

        private void ResetZoomLevel()
        {
            _lastFocusedTextEditor.FontSize = UserSettings.defaultFontSize;
        }
        private void ToolTipCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ToolTipCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CodeBox_TextArea_TextEntering(Key.F1,null);
        }
        
    }
}