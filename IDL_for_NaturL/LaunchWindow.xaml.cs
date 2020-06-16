using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;

namespace IDL_for_NaturL
{
    public partial class LaunchWindow : Window
    {
        MainWindow mainWindow;

        public LaunchWindow()
        {
            InitializeComponent();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("resources/lastfiles.txt", "");
            mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
            bool opened = mainWindow.NewFile(sender, e);
            if (opened)
                mainWindow.RemoveTab(0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("resources/lastfiles.txt", "");
            mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
            bool opened = mainWindow.Open_Click();
            if (opened)
            {
                mainWindow.RemoveTab(0);
            }
        }

        private void Window_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Open_Recent(object sender, RoutedEventArgs e)
        {
            mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
        }

        private void Minimize_Window(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}