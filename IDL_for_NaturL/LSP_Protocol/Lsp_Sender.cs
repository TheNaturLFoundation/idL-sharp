using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public interface LspSender
    {
        void RequestDefinition(Position position, string uri);
        void RequestKeywords(Position position, string uri);
        void Initialize();
        void DidOpen(string uri, string language, int version, string text);
        void DidChange(VersionedTextDocumentIdentifier versionedTextDocumentIdentifier, 
            List<TextDocumentContentChangeEvent> contentchangesEvents);
        void DidClose(string uri);
        
    }
}