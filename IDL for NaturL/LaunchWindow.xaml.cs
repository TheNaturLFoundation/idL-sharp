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
            File.WriteAllText("../../../ressources/lastfiles.txt", "");
            mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
            mainWindow.NewFile(sender, e);
            mainWindow.RemoveTab(0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("../../../ressources/lastfiles.txt", "");
            mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
            mainWindow.Open_Click();
            mainWindow.RemoveTab(0);
        }

        private void Window_MouseDown(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Open_Recent(object sender, RoutedEventArgs e)
        {
            mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void Minimize_Window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}