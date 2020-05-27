namespace IDL_for_NaturL
{
    public class Initialize_Params
    {
        public int processId;
        public string rootUri;
        public ClientCapabilities capabilities;
        public string initializationOptions;

        public Initialize_Params(int processId, string rootUri, ClientCapabilities capabilities, string initializationOptions)
        {
            this.processId = processId;
            this.rootUri = rootUri;
            this.capabilities = capabilities;
            this.initializationOptions = initializationOptions;
        }
    }
}