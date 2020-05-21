using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public interface LspSender
    {
        void RequestDefinition(Position position, string uri);
        void RequestKeywords(Position position, string uri);
        void InitializeRequest(int processId, string rootUri, ClientCapabilities capabilities);
        void ShutDownNotification();
        void DidOpenNotification(string uri, string language, int version, string text);
        void DidChangeNotification(VersionedTextDocumentIdentifier versionedTextDocumentIdentifier, 
            List<TextDocumentContentChangeEvent> contentchangesEvents);
        void DidCloseNotification(string uri);
        
    }
}