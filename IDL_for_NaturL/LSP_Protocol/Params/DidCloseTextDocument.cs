namespace IDL_for_NaturL
{
    public interface DidCloseTextDocument
    {
        
    }

    public class ConcreteDidCloseTextDocument : DidCloseTextDocument
    {
        private TextDocumentIdentifier _textDocumentIdentifier;

        public ConcreteDidCloseTextDocument(TextDocumentIdentifier textDocumentIdentifier)
        {
            _textDocumentIdentifier = textDocumentIdentifier;
        }
    }
}