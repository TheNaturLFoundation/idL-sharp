using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;

namespace IDL_for_NaturL
{
    // This class is not used at the moment, will be usefull later
    // Opens an input dialog and asks the user for an input
    public partial class InputWindow : Window
    {
        public TextBox Input
        {
            get => input;
            set => input = value;
        }

        private bool cancelled;

        public bool Cancelled1
        {
            get => cancelled;
            set => cancelled = value;
        }

        public InputWindow()
        {
            InitializeComponent();
        }
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e?.Key)
            {
                Created(sender, null);
            }
        }
        private void Created(object sender, RoutedEventArgs e)
        {
            if (input.Text.EndsWith(".ntl"))
            {
                var myfile = File.Create(input.Text);
                myfile.Close();
            }
            else
            {
                var myfile = File.Create(input.Text + ".ntl");
                myfile.Close();
            }
            this.Close();
        }
        private void Cancelled(object sender, RoutedEventArgs e)
        {
            this.Close();
            cancelled = true;
        }

        private void Cancelled_ESC(object sender, KeyEventArgs e)
        {
            if (Key.Escape == e?.Key)
            {
                Console.WriteLine("Escape");
                this.Close();
                cancelled = true;
            }
        }
    }
}