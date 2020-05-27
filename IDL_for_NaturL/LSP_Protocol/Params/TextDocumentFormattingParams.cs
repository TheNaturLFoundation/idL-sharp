namespace IDL_for_NaturL
{
    public class TextDocumentFormattingParams
    {
        public TextDocumentIdentifier textDocument;
        public FormattingOptions options;

        public TextDocumentFormattingParams(TextDocumentIdentifier textDocument, FormattingOptions options)
        {
            this.textDocument = textDocument;
            this.options = options;
        }
    }
}