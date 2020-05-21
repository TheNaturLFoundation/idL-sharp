namespace IDL_for_NaturL
{
    public class Initialize_Request
    {
        private int processId;
        private string rootUri;
        private ClientCapabilities capabilities;

        public Initialize_Request(int processId, string rootUri, ClientCapabilities capabilities)
        {
            this.processId = processId;
            this.rootUri = rootUri;
            this.capabilities = capabilities;
        }
    }
}