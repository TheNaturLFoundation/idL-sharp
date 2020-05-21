namespace IDL_for_NaturL
{
    public class ClientCapabilities
    {
        private TextDocumentClientCapabilities textDocument;

        public ClientCapabilities(TextDocumentClientCapabilities textDocument)
        {
            this.textDocument = textDocument;
        }
    }

    public class TextDocumentClientCapabilities
    {
        private CompletionClientCapabilities completion;
        private DefinitionClientCapabilities definition;
        private PublishDiagnosticsClientCapabilities publishDiagnostics;

        public TextDocumentClientCapabilities(CompletionClientCapabilities completion,
            DefinitionClientCapabilities definition,
            PublishDiagnosticsClientCapabilities publishDiagnostics)
        {
            this.completion = completion;
            this.definition = definition;
            this.publishDiagnostics = publishDiagnostics;
        }
    }

    public class CompletionClientCapabilities
    {
    }
    public class DefinitionClientCapabilities
    {
    }

    public class PublishDiagnosticsClientCapabilities
    {
    }
}