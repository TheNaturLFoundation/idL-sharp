namespace IDL_for_NaturL
{
    public struct TextDocument
    {
        public string uri { get; } 
        public string languageId { get; }
        public int version { get; }
        public string text { get; }

        public TextDocument(string uri, string languageId, int version, string text)
        {
            this.uri = uri;
            this.languageId = languageId;
            this.version = version;
            this.text = text;
        }
    }
}