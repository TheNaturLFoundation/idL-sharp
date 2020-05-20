namespace IDL_for_NaturL
{
    public interface DidOpenTextDocument
    {
        TextDocumentItem textDocumentItem { get; set; }
    }

    public class ConcreteDidOpenTextDocument : DidOpenTextDocument
    {
        public TextDocumentItem textDocumentItem { get; set; }

        public ConcreteDidOpenTextDocument(TextDocumentItem textDocumentItem)
        {
            this.textDocumentItem = textDocumentItem;
        }
    }
}