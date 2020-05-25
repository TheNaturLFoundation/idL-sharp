namespace IDL_for_NaturL
{
    public interface DidCloseTextDocument
    {
        
    }

    public class ConcreteDidCloseTextDocument : DidCloseTextDocument
    {
        public TextDocumentIdentifier textDocument;

        public ConcreteDidCloseTextDocument(TextDocumentIdentifier textDocument)
        {
            this.textDocument = textDocument;
        }
    }
}