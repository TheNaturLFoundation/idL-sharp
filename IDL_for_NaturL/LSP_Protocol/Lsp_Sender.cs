using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public interface LspSender
    {
        void RequestDefinition(Position position, string uri);
        void RequestKeywords(Position position, string uri);
        void InitializeRequest(int processId, string rootUri, ClientCapabilities capabilities);
        void ExitNotification();
        void ShutDownRequest();
        void DidOpenNotification(string uri, string language, int version, string text);

        void DidChangeNotification(VersionedTextDocumentIdentifier versionedTextDocumentIdentifier,
            IEnumerable<TextDocumentContentChangeEvent> contentchangesEvents);

        void DidCloseNotification(string uri);

        void FormattingRequest(string uri, int tabSize, bool insertSpaces);
    }
}