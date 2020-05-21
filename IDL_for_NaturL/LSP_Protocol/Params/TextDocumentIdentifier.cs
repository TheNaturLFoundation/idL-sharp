namespace IDL_for_NaturL
{
    public interface TextDocumentIdentifier
    {
        string uri { get; }
    }

    public class ConcreteTextDocumentIdentifier : TextDocumentIdentifier
    {
        public string uri { get; }

        public ConcreteTextDocumentIdentifier(string uri)
        {
            this.uri = uri;
        }
    }
}