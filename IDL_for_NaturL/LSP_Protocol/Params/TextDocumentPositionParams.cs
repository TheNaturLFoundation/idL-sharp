using System.Net.Mime;

namespace IDL_for_NaturL
{
    public interface TextDocumentPositionParams
    {
        TextDocumentIdentifier textDocument { get; set; }
        Position position { get; set; }
    }
}