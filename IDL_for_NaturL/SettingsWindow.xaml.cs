using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace IDL_for_NaturL
{
    public partial class SettingsWindow : Window
    {
        public string selected_item = "Mots clefs";
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
        

        public void Color_Changed(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color_Picker.Color);
            switch(selected_item)
            {
                case "Mots clefs":
                    Color_keywords.Foreground = brush;
                    Bold_Keywords.Foreground = brush;
                    break;
                case "Fonctions":
                    Color_functions.Foreground = brush;
                    Bold_Functions.Foreground = brush;
                    break;
                case "Types":
                    Color_types.Foreground = brush;
                    Bold_Types.Foreground = brush;
                    break;
                case "Vrai Faux":
                    Color_truefalse.Foreground = brush;
                    Bold_TrueFalse.Foreground = brush;
                    break;
            }
        }

        private void Selected_Combo_OnSelected(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem) Selected_Combo.SelectedItem;
            string value = typeItem.Content.ToString();
            selected_item = value;
        }
        
        private void Bold_Keywords_OnChecked(object sender, RoutedEventArgs e)
        {
            if (Bold_Keywords.IsChecked != null && (bool) Bold_Keywords.IsChecked)
            {
            }
        }

        private void Bold_Types_OnChecked(object sender, RoutedEventArgs e)
        {
            if (Bold_Types.IsChecked != null && (bool) Bold_Types.IsChecked)
            {
                
            }
        }

        private void Bold_TrueFalse_OnChecked(object sender, RoutedEventArgs e)
        {
            if (Bold_TrueFalse.IsChecked != null && (bool) Bold_TrueFalse.IsChecked)
            {
                
            }
        }

        private void Bold_Functions_OnChecked(object sender, RoutedEventArgs e)
        {
            if (Bold_Functions.IsChecked != null && (bool) Bold_Functions.IsChecked)
            {
                
            }
        }
    }
}