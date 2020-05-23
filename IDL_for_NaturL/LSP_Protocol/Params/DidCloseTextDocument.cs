namespace IDL_for_NaturL
{
    public interface DidCloseTextDocument
    {
        
    }

    public class ConcreteDidCloseTextDocument : DidCloseTextDocument
    {
        public TextDocumentIdentifier _textDocumentIdentifier;

        public ConcreteDidCloseTextDocument(TextDocumentIdentifier textDocumentIdentifier)
        {
            _textDocumentIdentifier = textDocumentIdentifier;
        }
    }
}