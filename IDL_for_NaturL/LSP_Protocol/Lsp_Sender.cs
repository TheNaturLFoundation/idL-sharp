namespace IDL_for_NaturL
{
    public interface LspSender
    {
        void RequestDefinition(Position position, string path);
        void RequestKeywords(Position position, string path);
        void Initialize();
        void DidOpen(string uri, string language, int version, string text);
        void DidChange();
        void DidClose();
        
    }
}