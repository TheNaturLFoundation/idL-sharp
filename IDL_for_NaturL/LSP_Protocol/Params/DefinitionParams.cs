using System.Net.Mime;

namespace IDL_for_NaturL
{
    public interface DefinitionParams : TextDocumentPositionParams
    {
        
    }

    public class ConcreteDefinitionParams : DefinitionParams
    {
        public TextDocumentIdentifier textDocument { get; set; }
        public Position position { get; set; }

        public ConcreteDefinitionParams(TextDocumentIdentifier textDocument, Position position)
        {
            this.textDocument = textDocument;
            this.position = position;
        }
    }
}