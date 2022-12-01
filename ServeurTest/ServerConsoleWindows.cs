using PATHServer;
using SocketLibrary;
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
            Server s = new Server();
            s.OnServerLog += Server_OnServerLog;
            return s;
        }

        private static void Server_OnServerLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}