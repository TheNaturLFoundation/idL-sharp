using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.VisualBasic;

namespace IDL_for_NaturL
{
    public partial class MainWindow
    {
        
    }
    class LineColorizer : DocumentColorizingTransformer
    {
        int lineNumber;
        private DiagnosticSeverity DiagnosticSeverity;
        public LineColorizer(int lineNumber, DiagnosticSeverity diagnosticSeverity)
        {
            this.lineNumber = lineNumber;
            this.DiagnosticSeverity = diagnosticSeverity;
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
            element.TextRunProperties.SetForegroundBrush(Brushes.Red);
            if (DiagnosticSeverity == DiagnosticSeverity.Information)
            {
                brush = (Brush) converter.ConvertFrom("#f7da00");
                element.TextRunProperties.SetForegroundBrush(Brushes.Black);
            }
            brush.Opacity = 0.3;
            element.TextRunProperties.SetBackgroundBrush(brush);
        }
    }
}