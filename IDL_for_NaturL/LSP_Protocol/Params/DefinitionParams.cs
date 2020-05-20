namespace IDL_for_NaturL
{
    public interface DefinitionParams : TextDocumentPositionParams
    {
        
    }

    public class ConcreteDefinitionParams : DefinitionParams
    {
        public string TextDocumentIdentifier { get; set; }
        public Position position { get; set; }

        public ConcreteDefinitionParams(string textDocumentIdentifier, Position position)
        {
            TextDocumentIdentifier = textDocumentIdentifier;
            this.position = position;
        }
    }
}