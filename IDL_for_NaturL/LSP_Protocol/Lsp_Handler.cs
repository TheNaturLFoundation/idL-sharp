using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDL_for_NaturL
{
    public class LspHandler : LspSender
    {
        public int id;
        private Process server;
        private Dictionary<int, string> idDictionary = new Dictionary<int, string>();
        public bool inHeader;
        
        public LspHandler(LspReceiver lspReceiver, Process server)
        {
            this.lspReceiver = lspReceiver;
            server.Start();
            server.ErrorDataReceived += (o, e) => Console.WriteLine(e.Data);  
            server.OutputDataReceived += ReceiveData;
            server.BeginOutputReadLine();
            server.BeginErrorReadLine();
            this.server = server;
        }

        private LspReceiver lspReceiver;
        
        public void RequestDefinition(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage
            {
                id = ++id, method = "textDocument/definition", parameters = 
                    new ConcreteDefinitionParams(uri,position)
            };
            idDictionary.Add(id,"textDocument/definition");
            string json = JsonConvert.SerializeObject(newMessage);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
        }

        public void RequestKeywords(Position position, string uri)
        {
            RequestMessage newMessage = new RequestMessage
            {
                id = ++id, method = "textDocument/completion", parameters = 
                    new ConcreteDefinitionParams(uri,position)
            };
            idDictionary.Add(id,"textDocument/completion");
            string json = JsonConvert.SerializeObject(newMessage);
            server.StandardInput.WriteLine(json);
            server.StandardInput.Flush();
            Console.WriteLine(json);
        }

        public void ReceiveData(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                inHeader = false;
                return;
            }
            // Everything will be received here
            string data = e.Data.Replace("{", "{\n").
                Replace("}","}\n").
                Replace(",",",\n").
                Replace("[","[\n").
                Replace("]","]\n");
            Console.WriteLine(data);
            JObject receivedData = (JObject) JsonConvert.DeserializeObject(data);
            if (IsPropertyExist(receivedData, "id"))
            {
                Console.WriteLine("Exists id");
                // It is a response if we get there.
                // Now need to find the id of the method called that was previously serialized
                
                idDictionary.TryGetValue(receivedData["id"].Value<int>(), out string method);
                switch (method)
                {
                    case "textDocument/definition":
                        Thread.Sleep(100);
                        lspReceiver.JumpToDefinition(receivedData["result"].ToObject<Location>());
                        break;
                    case "textDocument/completion":
                        Thread.Sleep(100);
                        JArray items = (JArray) receivedData["params"];
                        lspReceiver.Completion(items.ToObject<IList<CompletionItem>>());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (IsPropertyExist(receivedData, "method"))
            {
                Console.WriteLine("Exists Method");
                // It is a notification if we get there.
                switch (receivedData["method"].Value<string>())
                {
                    // Switch on methods in order to call the one sent by the notification
                    case "":
                        break;
                }
            }
            else
            {
                Console.WriteLine("No case");
            }

            inHeader = true;
        }
        public static bool IsPropertyExist(JObject settings, string name)
        {
            return settings[name] != null;
        }
    }
}