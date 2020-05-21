namespace IDL_for_NaturL
{
    public struct TextDocumentContentChangeEvent
    {
        public string text;

        public TextDocumentContentChangeEvent(string text)
        {
            this.text = text;
        }
    }
}