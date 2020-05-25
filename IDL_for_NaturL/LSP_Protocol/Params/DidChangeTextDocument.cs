using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public interface DidChangeTextDocument
    {
    }

    public class ConcreteDidChangeTextDocument : DidChangeTextDocument
    {
        public VersionedTextDocumentIdentifier textDocument { get; set; }

        public IEnumerable<TextDocumentContentChangeEvent> contentChanges { get; set; }
        public ConcreteDidChangeTextDocument(VersionedTextDocumentIdentifier textDocument,
            IEnumerable<TextDocumentContentChangeEvent> contentChanges)
        {
            this.textDocument = textDocument;
            this.contentChanges = contentChanges;
        }
    }
}