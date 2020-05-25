namespace IDL_for_NaturL
{
    public interface DidOpenTextDocument
    {
        TextDocument textDocument { get; set; }
    }

    public class ConcreteDidOpenTextDocument : DidOpenTextDocument
    {
        public TextDocument textDocument { get; set; }

        public ConcreteDidOpenTextDocument(TextDocument textDocument)
        {
            this.textDocument = textDocument;
        }
    }
}