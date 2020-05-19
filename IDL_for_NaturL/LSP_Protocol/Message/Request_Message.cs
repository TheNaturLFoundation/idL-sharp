namespace IDL_for_NaturL
{
    public class RequestMessage : Message
    {
        public int id;
        public dynamic? parameters; // params
        public string method;
    }
}