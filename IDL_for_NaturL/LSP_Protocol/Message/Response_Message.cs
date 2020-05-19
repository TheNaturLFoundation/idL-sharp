namespace IDL_for_NaturL
{
    public static class Response_Code
    {
        public const int ParseError = -32700;
        public const int InvalidRequest = -32600;
        public const int MethodNotFound = -32601;
        public const int InvalidParams = -32602;
        public const int InternalError = -32603;
        public const int serverErrorStart = -32099;
        public const int serverErrorEnd = -32000;
        public const int ServerNotInitialized = -32002;
        public const int UnknownErrorCod = -32001;
        public const int RequestCancelled = -32800;
        public const int ContentModified = -32801;
    }
    public class Response_Error
    {
        public int code;
        public string message;
        public dynamic? data;
    }

    public class Response_Message : Message
    {
        public int id;
        public Response_Error? Error;
        public dynamic? result;
    }
}