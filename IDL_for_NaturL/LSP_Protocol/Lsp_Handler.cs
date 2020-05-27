using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using IDL_for_NaturL.filemanager;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDL_for_NaturL
{
    public class LspHandler : LspSender
    {
        public int id;
        public bool initializedServer = false;
        private Process server;
        private Dictionary<int, string> idDictionary = new Dictionary<int, string>();
        public int breakCount = 0;


        public LspHandler(LspReceiver lspReceiver, Process server)
        {
            this.lspReceiver = lspReceiver;
            server.Start();
            server.OutputDataReceived += ReceiveData;
            server.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
            server.BeginOutputReadLine();
            server.BeginErrorReadLine();
            this.server = server;
        }

        private LspReceiver lspReceiver;

        public void RequestDefinition(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage(++id,
                new ConcreteDefinitionParams(new ConcreteTextDocumentIdentifier(uri), position),
                "textDocument/definition");

            idDictionary.Add(id, "textDocument/definition");
            string json = JsonConvert.SerializeObject(newMessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void RequestKeywords(Position position, string uri)
        {
            Console.WriteLine("request keywords: " + uri);
            RequestMessage newMessage = new RequestMessage
            (
                ++id,
                new ConcreteCompletionParams(new ConcreteTextDocumentIdentifier(uri), position),
                "textDocument/completion"
            );
            idDictionary.Add(id, "textDocument/completion");
            string json = JsonConvert.SerializeObject(newMessage);
            string headerAndJson = "Content-Length: " + json.Length + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void InitializeRequest(int processId, string uri, ClientCapabilities capabilities)
        {
            Initialize_Params initializeParams =
                new Initialize_Params(processId, uri, capabilities, UserSettings.language.ToStringRepresentation());
            RequestMessage newmessage = new RequestMessage(++id, initializeParams, "initialize");
            idDictionary.Add(id, "initialize");
            string json = JsonConvert.SerializeObject(newmessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            Console.WriteLine(headerAndJson);
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void InitializedNotification()
        {
            InitializedNotification initRequest =
                new InitializedNotification();
            Notification_Message initializeNotification =
                new Notification_Message("initialized", initRequest);
            string json = JsonConvert.SerializeObject(initializeNotification);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void ExitNotification()
        {
            ExitNotification exitNotification =
                new ExitNotification();
            Notification_Message notificationMessage = new Notification_Message("exit", exitNotification);
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void ShutDownRequest()
        {
            ShutDownParams initializeParams =
                new ShutDownParams();
            RequestMessage newmessage =
                new RequestMessage(++id, initializeParams, "shutdown");
            idDictionary.Add(id, "shutdown");
            string json = JsonConvert.SerializeObject(newmessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void DidOpenNotification(string uri, string language, int version, string text)
        {
            Console.WriteLine("Did open: " + uri);
            text = text.Replace("\r", "");
            DidOpenTextDocument document =
                new ConcreteDidOpenTextDocument(new TextDocument(uri, language, version, text));
            Notification_Message notificationMessage =
                new Notification_Message("textDocument/didOpen", document);
            
            string json = JsonConvert.SerializeObject(notificationMessage);
            if (json.Length > 4096)
            {
                document =
                    new ConcreteDidOpenTextDocument(new TextDocument(uri, language, version, ""));
                notificationMessage =
                    new Notification_Message("textDocument/didOpen", document);
                json = JsonConvert.SerializeObject(notificationMessage);
            }
            string headerAndJson = "Content-Length: " + json.Length + "\r\n\r\n" + json;
            Console.WriteLine("Header length did open: " + json.Length);
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
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
            if (json.Length > 4096)
            {
                contentchangesEvents = new List<TextDocumentContentChangeEvent> {new TextDocumentContentChangeEvent("")};
                document =
                    new ConcreteDidChangeTextDocument(versionedTextDocumentIdentifier,
                        contentchangesEvents);
                notificationMessage =
                    new Notification_Message("textDocument/didChange", document);
                json = JsonConvert.SerializeObject(notificationMessage);
            }
            string headerAndJson = "Content-Length: " + json.Length + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }
        
        public void DidCloseNotification(string uri)
        {
            Console.WriteLine("Did close uri: " + uri);
            DidCloseTextDocument document =
                new ConcreteDidCloseTextDocument(new ConcreteTextDocumentIdentifier(uri));
            Notification_Message notificationMessage =
                new Notification_Message("textDocument/didClose", document);
            string json = JsonConvert.SerializeObject(notificationMessage);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void FormattingRequest(string uri, int tabSize, bool insertSpaces)
        {
            TextDocumentFormattingParams textDocument =
                new TextDocumentFormattingParams(new ConcreteTextDocumentIdentifier(uri),
                    new FormattingOptions(tabSize, insertSpaces));
            RequestMessage message = new RequestMessage(++id, textDocument, "textDocument/formatting");
            idDictionary.Add(id, "textDocument/formatting");
            string json = JsonConvert.SerializeObject(message);
            string headerAndJson = "Content-Length: " + (json.Length) + "\r\n\r\n" + json;
            server.StandardInput.Write(headerAndJson);
            server.StandardInput.Flush();
        }

        public void ReceiveData(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                breakCount++;
                return;
            }

            if (breakCount < 2)
            {
                breakCount = 0;
                return;
            }

            breakCount = 0;
            // Everything will be received here
            string data = e.Data.Replace("{", "{\n").Replace("}", "}\n").Replace(",", ",\n").Replace("[", "[\n")
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
                            Console.WriteLine("Constant Keywords");
                            lspReceiver.Completion(new List<CompletionItem>());
                        }
                
                        break;
                    case "textDocument/formatting":
                        items = (JArray) receivedData["result"];
                        Console.WriteLine(receivedData);
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