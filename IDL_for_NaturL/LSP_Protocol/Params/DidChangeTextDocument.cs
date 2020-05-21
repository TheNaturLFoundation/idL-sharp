using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public interface DidChangeTextDocument
    {
    }

    public class ConcreteDidChangeTextDocument : DidChangeTextDocument
    {
        public VersionedTextDocumentIdentifier DocumentIdentifier { get; set; }

        public List<TextDocumentContentChangeEvent> TextDocumentContentChangeEvents { get; set; }
        public ConcreteDidChangeTextDocument(VersionedTextDocumentIdentifier documentIdentifier,
            List<TextDocumentContentChangeEvent> textDocumentContentChangeEvents)
        {
            DocumentIdentifier = documentIdentifier;
            TextDocumentContentChangeEvents = textDocumentContentChangeEvents;
        }
    }
}