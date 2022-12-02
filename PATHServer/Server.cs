using SocketLibrary;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace PATHServer
{
    public class Server
    {
        private readonly ServerSocket _server;

        public const int PORT = 8080;
        public const string ip_server = "127.0.0.1";

        public delegate void ServerLog(string log);

        public event ServerLog? OnServerLog;

        public Server()
        {
            _server = new ServerSocket(ip_server, PORT);
            _server.ServerStarted = ServerStarted;
            _server.AcceptedClient = AcceptedClient;
            _server.RecieveNameClient = RecieveNameClient;
            _server.RecieveData = RecieveData;
            _server.ClientDisconnected = ClientDisconnected;
            _server.ErrorLog = ErrorLog;
        }
#if DEBUG
        public void StartTest()
        {
            StartSockerServer();
            //UpdateData();
        }

#endif

        public void Start()
        {
            // Vérifier si on a les crédential de connexion WIFI

            // Si oui : 
            ConnectToWifi();

            // si non : 
            SearchAndWaitingClientInfo();
        }

        public void Stop()
        {
            // Dispose Socket connexion
        }   

        private void ConnectToWifi()
        {
        }

        private void SearchAndWaitingClientInfo()
        {
            // activé le HotPost WIFI 
            // attendre une connexion avec l'application
            // echange des credentials WIFI
            ConnectToWifi();
        }

        #region BDD
        /*
        private bool UpdateData()
        {
            string dbName = "TestDatabase.db";
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }
            using (var dbContext = new MyDbContext())
            {
                //Ensure database is created
                dbContext.Database.EnsureCreated();
                if (!dbContext.Sensors.Any())
                {
                    dbContext.Sensors.AddRange(new SensorData[]
                    {
                             new SensorData{ SensorId=1, Data="8000", DateTimeAdd = DateTime.Now },
                        });
                    dbContext.SaveChanges();
                }
                foreach (var sensor in dbContext.Sensors)
                {
                    OnServerLog?.Invoke($"SensorId={sensor.SensorId}\t={sensor.Data}\t{sensor.DateTimeAdd}");
                }
            }
        }
        */
        #endregion

        #region SERVER

        private void StartSockerServer()
        {
            _server.Start();
        }

        private void ServerStarted()
        {
            OnServerLog?.Invoke("serveur started");
        }

        private void AcceptedClient(string ip_client)
        {
            OnServerLog?.Invoke("AcceptedClient : " + ip_client);
            _server.SendValidation(ip_client);
        }

        private void RecieveNameClient(string name, string ip_client)
        {
            OnServerLog?.Invoke("RecieveNameClient, ip_client : " + ip_client + ", name : " + name);
        }

        private void RecieveData(string ip_client, string data)
        {
            OnServerLog?.Invoke("RecieveData, ip_client : " + ip_client + ", data : " + data);
        }

        private void ClientDisconnected(string ip_client)
        {
            OnServerLog?.Invoke("ClientDisconnected, ip_client : " + ip_client);
        }

        private void ErrorLog(string error)
        {
            OnServerLog?.Invoke("ErrorLog, error : " + error);
        }

        #endregion

        #region WIFI

        private string ExecuteCommand(string arg)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = arg, };
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            string strOutput = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return strOutput;
        }

        #endregion
    }
}