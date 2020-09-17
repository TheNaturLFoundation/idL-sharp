using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using IDL_for_NaturL.filemanager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDL_for_NaturL
{
    public class LspHandler : LspSender
    {
        public int id;
        public bool initializedServer;
        private TcpManager tcpManager;
        private Dictionary<int, string> idDictionary = new Dictionary<int, string>();
        public int breakCount = 0;

        private const int Port = 9131;
        private const string Ip = "localhost";
        
        
        public LspHandler(LspReceiver lspReceiver, Process lspServer)
        {
            lspServer.Start();
            Thread.Sleep(100);
            this.lspReceiver = lspReceiver;
            tcpManager = new TcpManager(IPAddress.Loopback,Port);
            tcpManager.ConnectAsync();
            tcpManager.OnReceive = ReceiveData;
        }
        
        
        private LspReceiver lspReceiver;

        public void RequestDefinition(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage(++id,
                new ConcreteDefinitionParams(new ConcreteTextDocumentIdentifier(uri), position),
                "textDocument/definition");

            idDictionary.Add(id, "textDocument/definition");
            string json = JsonConvert.SerializeObject(newMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void RequestKeywords(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage
            (
                ++id,
                new ConcreteCompletionParams(new ConcreteTextDocumentIdentifier(uri), position),
                "textDocument/completion"
            );
            idDictionary.Add(id, "textDocument/completion");
            string json = JsonConvert.SerializeObject(newMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void InitializeRequest(int processId, string uri, ClientCapabilities capabilities)
        {
            Initialize_Params initializeParams =
                new Initialize_Params(processId, uri, capabilities, UserSettings.language.ToStringRepresentation());
            RequestMessage newmessage = new RequestMessage(++id, initializeParams, "initialize");
            idDictionary.Add(id, "initialize");
            string json = JsonConvert.SerializeObject(newmessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void InitializedNotification()
        {
            InitializedNotification initRequest =
                new InitializedNotification();
            Notification_Message initializeNotification =
                new Notification_Message("initialized", initRequest);
            string json = JsonConvert.SerializeObject(initializeNotification);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void ExitNotification()
        {
            ExitNotification exitNotification =
                new ExitNotification();
            Notification_Message notificationMessage = new Notification_Message("exit", exitNotification);
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void ShutDownRequest()
        {
            ShutDownParams initializeParams =
                new ShutDownParams();
            RequestMessage newmessage =
                new RequestMessage(++id, initializeParams, "shutdown");
            idDictionary.Add(id, "shutdown");
            string json = JsonConvert.SerializeObject(newmessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void DidOpenNotification(string uri, string language, int version, string text)
        {
            text = text.Replace("\r", "");
            DidOpenTextDocument document =
                new ConcreteDidOpenTextDocument(new TextDocument(uri, language, version, text));
            Notification_Message notificationMessage =
                new Notification_Message("textDocument/didOpen", document);
            
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void DidChangeNotification(VersionedTextDocumentIdentifier versionedTextDocumentIdentifier,
            IEnumerable<TextDocumentContentChangeEvent> contentchangesEvents)
        {            
            DidChangeTextDocument document =
                new ConcreteDidChangeTextDocument(versionedTextDocumentIdentifier,
                    contentchangesEvents);
            Notification_Message notificationMessage =
                new Notification_Message("textDocument/didChange", document);
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }
        
        public void DidCloseNotification(string uri)
        {
            DidCloseTextDocument document =
                new ConcreteDidCloseTextDocument(new ConcreteTextDocumentIdentifier(uri));
            Notification_Message notificationMessage =
                new Notification_Message("textDocument/didClose", document);
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void FormattingRequest(string uri, int tabSize, bool insertSpaces)
        {
            TextDocumentFormattingParams textDocument =
                new TextDocumentFormattingParams(new ConcreteTextDocumentIdentifier(uri),
                    new FormattingOptions(tabSize, insertSpaces));
            RequestMessage message = new RequestMessage(++id, textDocument, "textDocument/formatting");
            idDictionary.Add(id, "textDocument/formatting");
            string json = JsonConvert.SerializeObject(message);
            string headerAndJson = "Content-Length: " + (json.Length ) + "\r\n\r\n" + json;
            tcpManager.Send(headerAndJson);
        }

        public void ReceiveData(string e)
        {
            if (String.IsNullOrEmpty(e)) return; // Tkt c'est pour toi simon
            e = e.Split("\r\n\r\n")[1];

            breakCount = 0;
            // Everything will be received here
            string data = e.Replace("{", "{\n").Replace("}", "}\n")
                .Replace(",", ",\n").Replace("[", "[\n")
                .Replace("]", "]\n");
            JObject receivedData = (JObject) JsonConvert.DeserializeObject(data);
            bool error = false;
            if (IsPropertyExist(receivedData, "id"))
            {
                if (IsPropertyExist(receivedData, "error"))
                {
                    error = true;
                }
                // It is a response if we get there.
                // Now need to find the id of the method called that was previously serialized
                idDictionary.TryGetValue(receivedData["id"].Value<int>(), out string method);
                if (!initializedServer)
                {
                    if (method == "initialize")
                    {
                        initializedServer = true;
                        // Send initialized notification //
                        InitializedNotification();
                    }

                    return;
                }

                JArray items;
                switch (method)
                {
                    case "textDocument/definition":
                        if (!error)
                        {
                            lspReceiver.JumpToDefinition(receivedData["result"].ToObject<Location>());
                        }

                        break;
                    case "textDocument/completion":
                        items = (JArray) receivedData["result"];
                        if (!error)
                        {
                            lspReceiver.Completion(items.ToObject<IList<CompletionItem>>());
                        }
                        else
                        {
                            lspReceiver.Completion(new List<CompletionItem>());
                        }
                
                        break;
                    case "textDocument/formatting":
                        items = (JArray) receivedData["result"];
                        if (! error)
                        {
                            foreach (JToken jToken in items)
                            {
                                TextEdit textEdit = jToken.ToObject<TextEdit>();
                                lspReceiver.Reformat(textEdit.range, textEdit.newText);
                            }
                        }

                        break;
                    case "shutdown":
                        //ExitNotification();
                        break;
                    default:
                        return;
                }
            }
            else if (IsPropertyExist(receivedData, "method"))
            {
                // Server notifications
                switch (receivedData["method"].Value<string>())
                {
                    // Switch on methods in order to call the method sent by the notification
                    case "textDocument/publishDiagnostics":
                        PublishDiagnosticsParams @params =
                            receivedData["params"].ToObject<PublishDiagnosticsParams>();
                        lspReceiver.Diagnostic(@params.diagnostics, @params.uri);
                        
                        break;
                }
            }
        }

        public static bool IsPropertyExist(JObject settings, string name)
        {
            return settings[name] != null;
        }
    }

    internal static class ConsoleAllocator
    {
        [DllImport(@"kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport(@"kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport(@"user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SwHide = 0;
        const int SwShow = 5;


        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SwShow);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SwHide);
        }
    }
}