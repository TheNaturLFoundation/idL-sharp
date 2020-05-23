using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
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
        public bool inHeader;

        public LspHandler(LspReceiver lspReceiver, Process server)
        {
            this.lspReceiver = lspReceiver;
            server.Start();
            server.OutputDataReceived += ReceiveData;
            server.BeginOutputReadLine();
            this.server = server;
        }

        private LspReceiver lspReceiver;

        public void RequestDefinition(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage
            {
                id = ++id, method = "textDocument/definition", parameters =
                    new ConcreteDefinitionParams(new ConcreteTextDocumentIdentifier(uri), position)
            };
            idDictionary.Add(id, "textDocument/definition");
            string json = JsonConvert.SerializeObject(newMessage);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void RequestKeywords(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage
            {
                id = ++id, method = "textDocument/completion", parameters =
                    new ConcreteCompletionParams(new ConcreteTextDocumentIdentifier(uri), position)
            };
            idDictionary.Add(id, "textDocument/completion");
            string json = JsonConvert.SerializeObject(newMessage);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void InitializeRequest(int processId, string uri, ClientCapabilities capabilities)
        {
            Initialize_Request initializeRequest =
                new Initialize_Request(processId, uri, capabilities);
            idDictionary.Add(id, "initialize");
            string json = JsonConvert.SerializeObject(initializeRequest);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void InitializedNotification()
        {
            InitializedNotification initRequest =
                new InitializedNotification();
            string json = JsonConvert.SerializeObject(initRequest);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void ShutDownNotification()
        {
            ShutDownNotification shutDownNotification =
                new ShutDownNotification();
            string json = JsonConvert.SerializeObject(shutDownNotification);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void DidOpenNotification(string uri, string language, int version, string text)
        {
            DidOpenTextDocument document =
                new ConcreteDidOpenTextDocument(new TextDocumentItem(uri, language, version, text));
            string json = JsonConvert.SerializeObject(document);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void DidChangeNotification(VersionedTextDocumentIdentifier versionedTextDocumentIdentifier,
            List<TextDocumentContentChangeEvent> contentchangesEvents)
        {
            DidChangeTextDocument document =
                new ConcreteDidChangeTextDocument(versionedTextDocumentIdentifier,
                    contentchangesEvents);
            string json = JsonConvert.SerializeObject(document);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void DidCloseNotification(string uri)
        {
            DidCloseTextDocument document =
                new ConcreteDidCloseTextDocument(new ConcreteTextDocumentIdentifier(uri));
            string json = JsonConvert.SerializeObject(document);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void ReceiveData(object sender, DataReceivedEventArgs e)
        {
            //iteLine("Received data: " + e.Data);
            if (string.IsNullOrEmpty(e.Data))
            {
                inHeader = false;
                return;
            }

            // Everything will be received here
            string data = e.Data.Replace("{", "{\n").Replace("}", "}\n").Replace(",", ",\n").Replace("[", "[\n")
                .Replace("]", "]\n");
            JObject receivedData = (JObject) JsonConvert.DeserializeObject(data);
            if (IsPropertyExist(receivedData, "id"))
            {
                // It is a response if we get there.
                // Now need to find the id of the method called that was previously serialized

                idDictionary.TryGetValue(receivedData["id"].Value<int>(), out string method);
                //iteLine("Method: " + method);
                /*if (!initializedServer)
                {
                    if (method == "initialize")
                    {
                        initializedServer = true;
                        // Send initialized notification //
                        InitializedNotification();
                    }

                    return;
                }*/

                switch (method)
                {
                    case "textDocument/definition":
                        lspReceiver.JumpToDefinition(receivedData["result"].ToObject<Location>());
                        break;
                    case "textDocument/completion":
                        //Thread.Sleep(100);
                        JArray items = (JArray) receivedData["params"];
                        lspReceiver.Completion(items.ToObject<IList<CompletionItem>>());
                        break;
                    default:
                        return;
                }
            }
            else if (IsPropertyExist(receivedData, "method"))
            {
                // It is a notification if we get there.
                switch (receivedData["method"].Value<string>())
                {
                    // Switch on methods in order to call the one sent by the notification
                    case "textDocument/publishDiagnostics":
                        PublishDiagnosticsParams @params =
                            receivedData["params"].ToObject<PublishDiagnosticsParams>();
                        foreach (var diagnostic in @params.diagnostics)
                        {
                            lspReceiver.Diagnostic(diagnostic.range,
                                diagnostic.severity ?? DiagnosticSeverity.Information,
                                diagnostic.message, @params.uri);
                        }

                        break;
                }
            }

        }

        public static bool IsPropertyExist(JObject settings, string name)
        {
            return settings[name] != null;
        }
    }
}