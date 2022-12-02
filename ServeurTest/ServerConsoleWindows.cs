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
            server = CreateServer();
            server.StartTest();

            ManualResetEvent keepAlive = new ManualResetEvent(true);
            keepAlive.Reset();
            keepAlive.WaitOne();
        }
        static Server CreateServer()
        {
            return new Server();
        }

        private static void Server_OnServerLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}