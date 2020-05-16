using System;
using System.Runtime.Serialization;

namespace IDL_for_NaturL.filemanager
{
    /// <summary>
    /// Data contract Class which transforms data into objects of the Language and Severity enums,
    /// this class is completely serializable.
    /// </summary>
    [DataContract]
    public class SettingsManager : IExtensibleDataObject
    {
        [DataMember] public string language;
        [DataMember] public string severity;
        //TODO Change the way type attribute is managed.
        /// <summary>
        /// Constructor that takes string from the xml data contract to create the attributes.
        /// </summary>
        /// <param name="languageName"></param>
        /// <param name="warningSeverity"></param>
        public SettingsManager(string languageName, string warningSeverity)
        {
            language = languageName;
            severity = warningSeverity;
        }
        /// <summary>
        /// Get Severity element of the enum from the string attribute of the class.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public WarningSeverity GetSeverity()
        {
            switch (severity)
            {
                case "light":
                    return WarningSeverity.Light;
                case "medium":
                    return WarningSeverity.Medium;
                case "severe":
                    return WarningSeverity.Severe;
                default:
                    throw new ArgumentException("No such warning severity : " + severity);
            }
        }
        /// <summary>
        /// Get Language element of the enum from the string attribute of the class.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Language GetLanguage()
        {
            switch (language)
            {
                case "french":
                    return Language.French;
                case "english":
                    return Language.English;
                default:
                    throw new ArgumentException("No such language name : " + language);
            }
        }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}