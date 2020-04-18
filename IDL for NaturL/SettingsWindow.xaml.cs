using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDL_for_NaturL
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Save_Setting(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("Not implemented Save");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}