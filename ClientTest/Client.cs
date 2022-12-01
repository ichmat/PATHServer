// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using PATHServer;
using SocketLibrary;

internal class Client
{
    private static ManualResetEvent keepAlive = new ManualResetEvent(true);
    private static ClientSocket _client;

    private static void Main(string[] args)
    {
        Console.WriteLine("This is Client");
        Thread.Sleep(3000);
        Console.WriteLine("try connect");
        startClient();

        keepAlive.Reset();
        keepAlive.WaitOne();
    }

    static ClientSocket CreateClient(int port, string ip)
    {
        ClientSocket client = new(ip, port, "pc-cedric");
        client.ClientConnected = ClientConnected;
        client.ClientDisconnected = ClientDisconnected;
        client.InterruptedConnexion = InterruptedConnexion;
        client.ConnexionRefused = ConnexionRefused;
        client.RecieveData = RecieveData;
        client.IncommingData = IncommingData;
        client.error = Error;
        return client;
    }

    static void startClient()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry("localhost");

        // Get host related information.
        string ip = hostEntry.AddressList[1].ToString();
        var port = Server.PORT;
        _client = CreateClient(port, ip);

        if (!_client.TryStart())
        {
            Console.WriteLine("Fail connexion");
        }
        else
        {
        }
    }

    static void ClientConnected()
    {
        Console.WriteLine("Connected from the server");
        _client.SendName();
    }

    static void ClientDisconnected()
    {
        Console.WriteLine("Disconnected from the server");
    }

    static void InterruptedConnexion()
    {
        Console.WriteLine("InterruptedConnexion");
    }

    static void ConnexionRefused()
    {
        Console.WriteLine("Connexion refused from the server");
    }

    static void RecieveData(string ip_server, string data)
    {
        Console.WriteLine("RecieveData ");
    }

    static void IncommingData(int sizeActualMessage, int sizeFullMessage)
    {

    }

    static void Error(string error)
    {

    }
}