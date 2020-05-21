using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        class LineColorizer : DocumentColorizingTransformer
        {
            int lineNumber;

            public LineColorizer(int lineNumber)
            {
                this.lineNumber = lineNumber;
            }

            protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
            {
                if (!line.IsDeleted && line.LineNumber == lineNumber) {
                    ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
                }
            }

            void ApplyChanges(VisualLineElement element)
            {
                // This is where you do anything with the line
                BrushConverter converter = new BrushConverter();
                Brush brush = (Brush) converter.ConvertFrom("#FF0000");
                brush.Opacity = 0.3;
                element.TextRunProperties.SetBackgroundBrush(brush);
                element.TextRunProperties.SetForegroundBrush(Brushes.Red);
            }
        }
    }
}