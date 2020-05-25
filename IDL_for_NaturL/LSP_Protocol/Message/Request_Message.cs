using Newtonsoft.Json;

namespace IDL_for_NaturL
{
    public class RequestMessage : Message
    {
        public int id;
        [JsonProperty("params")]
        public dynamic? parameters; // params
        public string method;

        public RequestMessage(int id, dynamic parameters, string method)
        {
            this.id = id;
            this.parameters = parameters;
            this.method = method;
        }
    }
}