using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace IDL_for_NaturL
{
    
    public partial class MainWindow
    {
        public enum Language
        {
            English,
            French
        }

        public Language language;
        private void FrenchBoxClicked(object sender, RoutedEventArgs e)
        {
            if (FrenchBox.IsChecked)
            {
                EngBox.IsChecked = false;
                language = Language.French;
                UpdateLanguage(language);
            }
        }

        private void EngBoxClicked(object sender, RoutedEventArgs e)
        {
            if (FrenchBox.IsChecked)
            {
                FrenchBox.IsChecked = false;
                language = Language.English;
                UpdateLanguage(language);
            }
        }

        public void UpdateLanguage(Language language)
        {
            List<string> fileFrench = new List<string>()
            {
                "_Nouveau Fichier","_Nouvel onglet", "_Fermer l'onglet","_Ouvrir un fichier",
                "_Sauvegarder", "_Sauvegarder sous", "_Paramètres", "","_Quitter idL"
            };
            List<string>  editFrench = new List<string>()
            {
                "_Copier","_Coller", "_Couper","_Annuler","_Rétablir"
            };
            List<List<string>> frenchList = new List<List<string>>() {fileFrench,editFrench};
            
            List<string> fileEng = new List<string>()
            {
                "_New file","_New tab", "_Close tab","_Open file",
                "_Save", "_Save as", "_Settings", "","_Close idL"
            };
            List<string> editEng = new List<string>()
            {
                "_Copy","_Paste", "_Cut","_Undo","_Redo"
            };
            List<List<string>> engList = new List<List<string>>() {fileEng,editEng};
            Console.WriteLine(language);
            switch (language)
            {
                case Language.English:
                {
                    ((MenuItem) Menu.Items[0]).Header = "_Fichier";
                    ((MenuItem) Menu.Items[1]).Header = "_Editer";
                    ((MenuItem) Menu.Items[2]).Header = "_Langue";
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < ((MenuItem) Menu.Items[i]).Items.Count; j++)
                        {
                            if (j != 7)
                            {
                                ((MenuItem) ((MenuItem) Menu.Items[i]).Items[j]).Header = frenchList[i][j];
                            }
                        }
                    }
                } 
                    break;
                case Language.French:
                {
                    ((MenuItem) Menu.Items[0]).Header = "_File";
                    ((MenuItem) Menu.Items[1]).Header = "_Edit";
                    ((MenuItem) Menu.Items[2]).Header = "_Language";
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < ((MenuItem) Menu.Items[i]).Items.Count; j++)
                        {
                            if (j != 7)
                            {
                                ((MenuItem) ((MenuItem) Menu.Items[i]).Items[j]).Header = engList[i][j];
                            }
                        }
                    }
                }
                    break;
            }
        }
    }
}