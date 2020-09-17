using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpClient = NetCoreServer.TcpClient;

namespace IDL_for_NaturL
{
    public class TcpManager : TcpClient
    {
        public Action<string> OnReceive = _ => {};
        public TcpManager(IPAddress address, int port) : base(address, port)
        {
            
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int) offset, (int) size);
            OnReceive(message);
        }
        
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat SSL session caught an error with code {error}");
        }
    }
}