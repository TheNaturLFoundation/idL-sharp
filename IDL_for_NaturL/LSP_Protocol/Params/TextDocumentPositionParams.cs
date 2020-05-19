namespace IDL_for_NaturL
{
    public interface TextDocumentPositionParams
    {
        string TextDocumentIdentifier { get; set; }
        Position position { get; set; }
    }
}