namespace IDL_for_NaturL
{
    public interface LspSender
    {
        void RequestDefinition(Position position, string path);
        void RequestKeywords(Position position, string path);
    }
}