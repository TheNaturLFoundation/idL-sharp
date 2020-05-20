namespace IDL_for_NaturL
{
    public struct TextDocumentItem
    {
        public string uri { get; } 
        public string languageId { get; }
        public int version { get; }
        public string content { get; }

        public TextDocumentItem(string uri, string languageId, int version, string content)
        {
            this.uri = uri;
            this.languageId = languageId;
            this.version = version;
            this.content = content;
        }
    }
}