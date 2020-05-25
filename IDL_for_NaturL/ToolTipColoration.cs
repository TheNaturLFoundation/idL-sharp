using System.ComponentModel;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.VisualBasic;

namespace IDL_for_NaturL
{
    class LineColorizer : DocumentColorizingTransformer
    {
        private int lineNumber;
        private DiagnosticSeverity _diagnosticSeverity;
        public LineColorizer(int lineNumber, DiagnosticSeverity diagnosticSeverity)
        {
            this.lineNumber = lineNumber;
            this._diagnosticSeverity = diagnosticSeverity;
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
            Brush brush;
            switch (_diagnosticSeverity)
            {
                case DiagnosticSeverity.Error:
                    brush = (Brush) converter.ConvertFrom("#FF0000");
                    brush.Opacity = 0.3;
                    element.TextRunProperties.SetBackgroundBrush(brush);
                    break;
                case DiagnosticSeverity.Warning:
                     brush = (Brush) converter.ConvertFrom("#FFCC00");
                     brush.Opacity = 0.3;
                     element.TextRunProperties.SetBackgroundBrush(brush);
                     break;
            }
        }
    }
}