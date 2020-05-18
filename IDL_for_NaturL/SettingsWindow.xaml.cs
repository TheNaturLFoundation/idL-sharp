using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using IDL_for_NaturL.colorscheme;
using IDL_for_NaturL.filemanager;

namespace IDL_for_NaturL
{
    public partial class SettingsWindow : Window
    {
        public string selected_item = "Mots clefs";

        public SettingsWindow()
        {
            InitializeComponent();
        }

        //TODO Ajouter la possibilité de charger un fichier directement dans les configurations.
        private void Save_Setting(object sender, RoutedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("ressources/user_coloration.xshd");
            XmlNode root = doc.DocumentElement;
            XmlNodeList ruleSets = root.FirstChild.NextSibling.NextSibling.FirstChild.ChildNodes;
            foreach (XmlNode rule in ruleSets)
            {
                if (rule.Name.Equals("MarkPrevious"))
                {
                    try
                    {
                        rule.Attributes.GetNamedItem("color").Value = GetHexFromBrush(Color_functions);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                else
                {
                    var node = rule.Attributes?.GetNamedItem("name");
                    switch (node?.InnerText)
                    {
                        case "structure_words":
                            rule.Attributes.GetNamedItem("color").Value = GetHexFromBrush(Color_keywords);
                            break;
                        case "booleen":
                            rule.Attributes.GetNamedItem("color").Value = GetHexFromBrush(Color_truefalse);
                            break;
                        case "types":
                            rule.Attributes.GetNamedItem("color").Value = GetHexFromBrush(Color_types);
                            break;
                        default:
                            continue;
                    }
                }
            }
            Thread thread = new Thread(() => doc.Save("ressources/user_coloration.xshd"));
            thread.Start();
            UserSettings.syntaxFilePath = "ressources/user_coloration.xshd";
            MainWindow.Instance.UpdateColorScheme(doc);
        }

        private string GetHexFromBrush(TextBlock colorBlock)
        {
            Color color = ((SolidColorBrush) colorBlock.Foreground).Color;
            return color.ToHex();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Save_Setting(null, null);
            Cancel_Click(null, null);
        }


        public void Color_Changed(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color_Picker.Color);
            switch (selected_item)
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

        private void DefaultReset(object sender, RoutedEventArgs e)
        {
            UserSettings.syntaxFilePath = "ressources/naturl_coloration.xshd";
            XmlDocument doc = new XmlDocument();
            doc.Load("ressources/naturl_coloration.xshd");
            MainWindow.Instance.UpdateColorScheme(doc);
            Close();
        }

        private void TabablzControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}