namespace IDL_for_NaturL
{
    public class Initialize_Params
    {
        public int processId;
        public string rootUri;
        public ClientCapabilities capabilities;

        public Initialize_Params(int processId, string rootUri, ClientCapabilities capabilities)
        {
            this.processId = processId;
            this.rootUri = rootUri;
            this.capabilities = capabilities;
        }
    }
}