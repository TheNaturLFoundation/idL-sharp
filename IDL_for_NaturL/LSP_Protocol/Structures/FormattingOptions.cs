using System.Windows.Media.Animation;

namespace IDL_for_NaturL
{
    public struct FormattingOptions
    {
        public int tabSize;
        public bool insertSpaces;

        public FormattingOptions(int tabSize, bool insertSpaces)
        {
            this.tabSize = tabSize;
            this.insertSpaces = insertSpaces;
        }
    }
}