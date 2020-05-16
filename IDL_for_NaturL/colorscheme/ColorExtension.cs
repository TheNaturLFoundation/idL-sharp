using System.Drawing;
using Color = System.Drawing.Color;

namespace IDL_for_NaturL.colorscheme
{
    public static class ColorExtension
    {
        public static string ToHex(this System.Windows.Media.Color color)
        {
            return ColorTranslator.ToHtml(Color.FromArgb(color.A, color.R, color.G, color.B)).ToLower();
        } 
    }
}