using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using Dragablz;
using HighlightingManager = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;
namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        private void Window_Resize(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 50 && e.ClickCount == 1)
            {
                clickPosition = e.GetPosition(this).Y;
            }
            else
            {
                clickPosition = -1;
            }
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (position.Y < 50)
            {
                this.DragMove();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            double yVel = e.GetPosition(this).Y - clickPosition;
            if (yVel > 0 && clickPosition > 0 && WindowState == WindowState.Maximized && e.ClickCount == 1)
            {
                WindowState = WindowState.Normal;
            }
            else
                clickPosition = -1;
        }

        private void Double_Click(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            double y = p.Y;
            if (y < 50)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }
        
    }
}