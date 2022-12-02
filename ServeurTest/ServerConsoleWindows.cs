using PATHServer;
using SocketLibrary;
using System.Net;
using System.Net.WebSockets;

namespace ServeurTest
{
    public class ServerTest
    {
        private static Server server;

        static void Main(string[] args)
        {
            Console.WriteLine("This is Serveur");
            WebSocket webSocket = WebSocket.CreateFromStream();
            server = CreateServer();
            server.StartTest();

            ManualResetEvent keepAlive = new ManualResetEvent(true);
            keepAlive.Reset();
            keepAlive.WaitOne();
        }
        static Server CreateServer()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            Console.WriteLine(hostName);
            // Get the IP
            string myIP = Dns.GetHostByName(hostName).AddressList[1].ToString();
            Console.WriteLine("My IP Address is :" + myIP);
            Server s = new Server(myIP);
            s.OnServerLog += Server_OnServerLog;
            return s;
        }

        private static void Server_OnServerLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}