namespace IDL_for_NaturL
{
    public struct CompletionItem
    {
        public string label;
        public string detail;

        public CompletionItem(string label, string detail)
        {
            this.label = label;
            this.detail = detail;
        }
    }
}