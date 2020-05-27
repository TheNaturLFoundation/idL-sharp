using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace IDL_for_NaturL.filemanager
{
    /// <summary>
    /// Static class containing the values of the differents parameters used in the management
    /// of user settings throughout the utilisation of the idL
    /// </summary>
    public static class UserSettings
    {
        public static Language language;
        public static WarningSeverity warningSeverity;
        public static string syntaxFilePath;
        public static double defaultFontSize = 16;

        /// <summary>
        /// Function that loads the configuration from XML DataContract present in the class SettingsManager 
        /// </summary>
        /// <param name="filename"></param>
        public static void LoadUserSettings(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename,
                    FileMode.Open);
                XmlDictionaryReader reader =
                    XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(SettingsManager));

                // Deserialize the data and read it from the instance.
                SettingsManager deserializedSettingsManager =
                    (SettingsManager) ser.ReadObject(reader, true);
                reader.Close();
                fs.Close();
                language = deserializedSettingsManager.GetLanguage();
                warningSeverity = deserializedSettingsManager.GetSeverity();
                syntaxFilePath = deserializedSettingsManager.syntaxFilePath;
                defaultFontSize = deserializedSettingsManager.fontSize;
            }
            catch (Exception)
            {
                language = Language.French;
                warningSeverity = WarningSeverity.Light;
                syntaxFilePath = "resources/naturl_coloration.xshd";
                defaultFontSize = 18d;
            }
        }

        /// <summary>
        /// Function that saves and serialize the content of the UserSetting class into readable DataContract XML
        /// </summary>
        /// <param name="fileName"></param>
        public static void SaveUserSettings(string fileName)
        {
            SettingsManager S1 = new SettingsManager(language.ToStringRepresentation(),
                warningSeverity.ToStringRepresentation(), syntaxFilePath, MainWindow._lastFocusedTextEditor.FontSize);
            FileStream writer = new FileStream(fileName, FileMode.Create);
            DataContractSerializer ser =
                new DataContractSerializer(typeof(SettingsManager));
            ser.WriteObject(writer, S1);
            writer.Close();
        }
    }
}