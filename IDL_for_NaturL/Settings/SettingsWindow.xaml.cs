using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using IDL_for_NaturL.colorscheme;
using IDL_for_NaturL.filemanager;

namespace IDL_for_NaturL
{
    public partial class SettingsWindow
    {
        public bool clicked;
        public int selected_item = 0;
        public SettingsWindow Instance = null;

        public SettingsWindow()
        {
            InitializeComponent();
            UpdateAtLaunch();
        }

        //TODO Ajouter la possibilité de charger un fichier directement dans les configurations.
        private void UpdateAtLaunch()
        {
            UpdateLanguage(UserSettings.language);
            XmlDocument doc = new XmlDocument();
            doc.Load("resources/user_coloration.xshd");
            XmlNode root = doc.DocumentElement;
            XmlNodeList ruleSets = root.FirstChild.NextSibling.NextSibling.FirstChild.ChildNodes;
            foreach (XmlNode rule in ruleSets)
            {
                if (rule.Name.Equals("MarkPrevious"))
                {
                    Brush brush;
                    try
                    {
                        brush = new SolidColorBrush(
                            (Color) ColorConverter.ConvertFromString(rule.Attributes.GetNamedItem("color").Value));
                        Color_functions.Foreground = brush;
                    }
                    catch (Exception exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    var node = rule.Attributes?.GetNamedItem("name");
                    Brush brush;
                    switch (node?.InnerText)
                    {
                        case "structure_words":
                            brush = new SolidColorBrush(
                                (Color) ColorConverter.ConvertFromString(rule.Attributes.GetNamedItem("color").Value));
                            Color_keywords.Foreground = brush;
                            break;
                        case "booleen":
                            brush = new SolidColorBrush(
                                (Color) ColorConverter.ConvertFromString(rule.Attributes.GetNamedItem("color").Value));
                            Color_truefalse.Foreground = brush;
                            break;
                        case "types":
                            brush = new SolidColorBrush(
                                (Color) ColorConverter.ConvertFromString(rule.Attributes.GetNamedItem("color").Value));
                            Color_types.Foreground = brush;
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        private void UpdateLanguage(Language language)
        {
            if (language == IDL_for_NaturL.Language.French)
            {
                SaveButton.Content = "Sauvegarder";
                DefaultResetButton.Content = "Paramètres par défaut";
                CancelButton.Content = "Annuler";
                Color_keywords.Text = "Mots clés: \n fonction \n sinon_si \n si";
                Color_functions.Text = "Fonctions:\n afficher\n longueur ";
                Color_types.Text = "Types:\n entier\n booleen\n chaine";
                Color_truefalse.Text = "Booleens:\n vrai\n faux";
                Constantes.Content = "Constantes";
                Type.Content = "Type";
                Fonction.Content = "Fonction";
                MotClef.Content = "Mots clés";
            }
            else
            {
                SaveButton.Content = "Save";
                DefaultResetButton.Content = "Reset defaults";
                CancelButton.Content = "Cancel";
                Color_keywords.Text = "Keywords: \n fonction \n sinon_si \n si";
                Color_functions.Text = "Functions:\n afficher\n longueur ";
                Color_types.Text = "Types:\n entier\n booleen\n chaine";
                Color_truefalse.Text = "Booleans:\n vrai\n faux";
                Constantes.Content = "Constants";
                Type.Content = "Type";
                Fonction.Content = "Function";
                MotClef.Content = "Keywords";
            }
        }

        private void Save_Setting(object sender, RoutedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("resources/user_coloration.xshd");
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
                        // ignored
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

            Thread thread = new Thread(() => doc.Save("resources/user_coloration.xshd"));
            thread.Start();
            UserSettings.syntaxFilePath = "resources/user_coloration.xshd";
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

        public void MouseLeftDown(object sender, MouseEventArgs e)
        {
            clicked = true;
        }

        public void MouseLeftUp(object sender, MouseEventArgs e)
        {
            clicked = false;
        }

        public void Color_Changed(object sender, RoutedEventArgs e)
        {
            if (!clicked) return;
            SolidColorBrush brush = new SolidColorBrush(Color_Picker.Color);
            switch (selected_item)
            {
                case 0:
                    Color_keywords.Foreground = brush;
                    //Bold_Keywords.Foreground = brush;
                    break;
                case 1:
                    Color_functions.Foreground = brush;
                    //Bold_Functions.Foreground = brush;
                    break;
                case 2:
                    Color_types.Foreground = brush;
                    //Bold_Types.Foreground = brush;
                    break;
                case 3:
                    Color_truefalse.Foreground = brush;
                    //Bold_TrueFalse.Foreground = brush;
                    break;
            }
        }

        private void Selected_Combo_OnSelected(object sender, RoutedEventArgs e)
        {
            selected_item = Selected_Combo.SelectedIndex;
        }

        // These are functions that control the bold not implemented because of lack of time
        // This would have been useful in order to allow the user to chose if its text is bold or not
        private void Bold_Keywords_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (Bold_Keywords.IsChecked != null && (bool) Bold_Keywords.IsChecked)
            {
            }
        }

        private void Bold_Types_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (Bold_Types.IsChecked != null && (bool) Bold_Types.IsChecked)
            {
            }
        }

        private void Bold_TrueFalse_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (Bold_TrueFalse.IsChecked != null && (bool) Bold_TrueFalse.IsChecked)
            {
            }
        }

        private void Bold_Functions_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (Bold_Functions.IsChecked != null && (bool) Bold_Functions.IsChecked)
            {
            }
        }

        private void DefaultReset(object sender, RoutedEventArgs e)
        {
            UserSettings.syntaxFilePath = "resources/naturl_coloration.xshd";
            XmlDocument doc = new XmlDocument();
            doc.Load("resources/naturl_coloration.xshd");
            MainWindow.Instance.UpdateColorScheme(doc);
            Close();
        }
    }
}