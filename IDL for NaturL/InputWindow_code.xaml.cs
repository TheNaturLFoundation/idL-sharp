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

        private bool IsOpen;

        public bool IsOpen1
        {
            get => IsOpen;
            set => IsOpen = value;
        }

        public InputWindow()
        {
            InitializeComponent();
            IsOpen = true;
        }
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e?.Key)
            {
                Created(sender, null);
            }
            Console.WriteLine("input is: "+input.Text);
        }
        private void Created(object sender, RoutedEventArgs e)
        {
            if (input.Text.Contains(".ntl"))
            {
                File.Create(input.Text);
            }
            else
            {
                File.Create(input.Text + ".ntl");
            }
            this.Close();
            IsOpen = false;
        }
        private void Cancelled(object sender, RoutedEventArgs e)
        {
            this.Close();
            IsOpen = false;
        }
    }
}