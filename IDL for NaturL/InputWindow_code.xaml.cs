using System;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace IDL_for_NaturL
{
    // This class is not used at the moment, will be usefull later
    // Opens an input dialog and asks the user for an input
    public partial class InputWindow : Window
    {
        Action<string> _setValue;
        public InputWindow(Action<string> setValue)
        {
            _setValue = setValue;
            InitializeComponent();

        }
        private void KeyPressed(object sender, KeyEventArgs e = null)
        {
            if (Key.Enter == e?.Key)
            {
                _setValue?.Invoke (input.Text);
                if (input.Text.Contains(".ntl"))
                {
                    File.Create(input.Text);
                }
                else
                {
                    File.Create(input.Text + ".ntl");
                }
                this.Close();
            }
        }
        private void Created(object sender, RoutedEventArgs e)
        {
            if (_setValue != null)
                _setValue(input.Text);
            if (input.Text.Contains(".ntl"))
            {
                File.Create(input.Text);
            }
            else
            {
                File.Create(input.Text + ".ntl");
            }
            this.Close();
        }
        private void Cancelled(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}