using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls.Primitives;

namespace IDL_for_NaturL
{
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
            if (Key.Enter == e.Key)
            {
                if (_setValue != null)
                    _setValue (input.Text);
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