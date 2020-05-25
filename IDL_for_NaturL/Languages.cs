using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using IDL_for_NaturL.filemanager;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        private Language language;

        private void FrenchBoxClicked(object sender, RoutedEventArgs e)
        {
            if (EngBox.IsChecked)
            {
                EngBox.IsChecked = false;
                language = IDL_for_NaturL.Language.French;
                UpdateLanguage(language);
                UserSettings.language = language;
            }

            FrenchBox.IsChecked = true;
        }

        private void EngBoxClicked(object sender, RoutedEventArgs e)
        {
            if (FrenchBox.IsChecked)
            {
                FrenchBox.IsChecked = false;
                language = IDL_for_NaturL.Language.English;
                UpdateLanguage(language);
                UserSettings.language = language;
            }

            EngBox.IsChecked = true;
        }

        public void InitialiseLanguageComponents(Language language)
        {
            switch (language)
            {
                case IDL_for_NaturL.Language.English:
                    EngBox.IsChecked = true;
                    UpdateLanguage(language);
                    FrenchBox.IsChecked = false;
                    break;
                case IDL_for_NaturL.Language.French:
                    EngBox.IsChecked = false;
                    UpdateLanguage(language);
                    FrenchBox.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateLanguage(Language language)
        {
            List<string> fileFrench = new List<string>()
            {
                "_Nouveau Fichier", "_Nouvel onglet", "_Fermer l'onglet", "_Ouvrir un fichier",
                "_Sauvegarder", "_Sauvegarder sous", "_Paramètres", "", "_Quitter idL"
            };
            List<string> editFrench = new List<string>()
            {
                "_Copier", "_Coller", "_Couper", "_Annuler", "_Rétablir"
            };
            List<List<string>> frenchList = new List<List<string>>() {fileFrench, editFrench};

            List<string> fileEng = new List<string>()
            {
                "_New file", "_New tab", "_Close tab", "_Open file",
                "_Save", "_Save as", "_Settings", "", "_Close idL"
            };
            List<string> editEng = new List<string>()
            {
                "_Copy", "_Paste", "_Cut", "_Undo", "_Redo"
            };
            List<List<string>> engList = new List<List<string>>() {fileEng, editEng};
            switch (language)
            {
                case IDL_for_NaturL.Language.French:
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
                case IDL_for_NaturL.Language.English:
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