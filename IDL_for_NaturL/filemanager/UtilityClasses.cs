using System;
using System.Runtime.CompilerServices;

namespace IDL_for_NaturL
{
    public enum Language
    {
        English,
        French
    }

    public static class LanguageExtension
    {
        public static string ToStringRepresentation(this Language language)
        {
            switch (language)
            {
                case Language.English:
                    return "english";
                case Language.French:
                    return "french";
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }
    }
    public enum WarningSeverity
    {
        Light,
        Medium,
        Severe
    }
    public static class WarningSeverityExtension
    {
        public static string ToStringRepresentation(this WarningSeverity warningSeverity)
        {
            switch (warningSeverity)
            {
                case WarningSeverity.Light:
                    return "light";
                case WarningSeverity.Medium:
                    return "medium";
                case WarningSeverity.Severe:
                    return "severe";
                default:
                    throw new ArgumentOutOfRangeException(nameof(warningSeverity), warningSeverity, null);
            }
        }
    }
}