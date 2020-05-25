using Newtonsoft.Json;

namespace IDL_for_NaturL
{
    public class Notification_Message : Message
    {
        public string method;
        [JsonProperty("params")]
        public dynamic? parameters; // params

        public Notification_Message(string method, dynamic parameters)
        {
            this.method = method;
            this.parameters = parameters;
        }
    }
}