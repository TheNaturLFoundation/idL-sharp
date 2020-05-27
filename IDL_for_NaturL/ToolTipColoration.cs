using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;

namespace IDL_for_NaturL
{
    class LineColorizer : DocumentColorizingTransformer
    {
        private int lineNumber;
        private int startOffset;
        private DiagnosticSeverity _diagnosticSeverity;
        public LineColorizer(int lineNumber, DiagnosticSeverity diagnosticSeverity, int startOffset)
        {
            this.lineNumber = lineNumber;
            this._diagnosticSeverity = diagnosticSeverity;
            this.startOffset = startOffset;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
                try
                {
                    ChangeLinePart(startOffset, line.EndOffset, ApplyChanges);
                }
                catch (ArgumentOutOfRangeException e)
                { 
                    ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
                }
            }
        }

        void ApplyChanges(VisualLineElement element)
        {
            // This is where you do anything with the line
            BrushConverter converter = new BrushConverter();
            Brush brush;
            TextDecorationCollection myCollection;
            switch (_diagnosticSeverity)
            {
                case DiagnosticSeverity.Error:
                    brush = (Brush) converter.ConvertFrom("#FF0000");
                    brush.Opacity = 0.3;
                    TextDecoration myUnderline = new TextDecoration();
                    // Create a solid color brush pen for the text decoration.
                    myUnderline.Pen = new Pen(Brushes.Red, 1);
                    myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                    myCollection = new TextDecorationCollection();
                    myCollection.Add(myUnderline);
                    element.TextRunProperties.SetTextDecorations(myCollection);
                    break;
                case DiagnosticSeverity.Warning:
                    brush = (Brush) converter.ConvertFrom("#ed9200");
                    
                    myUnderline = new TextDecoration();
                    // Create a solid color brush pen for the text decoration.
                    myUnderline.Pen = new Pen(brush,1);
                    myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                    myCollection = new TextDecorationCollection();
                    myCollection.Add(myUnderline);
                    element.TextRunProperties.SetTextDecorations(myCollection);
                    break;
            }
        }
    }

    class STDColorizer : DocumentColorizingTransformer
    {
        private int lineNumber;
        private DiagnosticSeverity _diagnosticSeverity;

        public STDColorizer(int lineNumber, DiagnosticSeverity diagnosticSeverity)
        {
            this.lineNumber = lineNumber;
            this._diagnosticSeverity = diagnosticSeverity;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
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
                case DiagnosticSeverity.Warning:
                    element.TextRunProperties.SetForegroundBrush(Brushes.Orange);
                    break;
                case DiagnosticSeverity.Information:
                    break;
            }
        }
    }
}