namespace IDL_for_NaturL
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        public int version;
        public string uri { get; }
        public VersionedTextDocumentIdentifier(int version, string uri)
        {
            this.version = version;
            this.uri = uri;
        }

    }
}