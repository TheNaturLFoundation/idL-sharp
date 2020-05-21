namespace IDL_for_NaturL
{
    public interface CompletionParams : TextDocumentPositionParams
    {
        
    }

    public class ConcreteCompletionParams : CompletionParams
    {
        public TextDocumentIdentifier textDocument { get; set; }
        public Position position { get; set; }

        public ConcreteCompletionParams(TextDocumentIdentifier textDocument, Position position)
        {
            this.textDocument = textDocument;
            this.position = position;
        }
    }
}