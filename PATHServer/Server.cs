using PATHServer.BDD.Models;
using PATHServer.CommandEnv;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace PATHServer
{
    public class Server
    {
        public const int PORT = 8080;

        public delegate void ServerLog(string log);

        public event ServerLog? OnServerLog;

        public Server()
        {
            
        }
#if DEBUG
        public void StartTest()
        {
            SearchWifi();
            HttpClient client = new HttpClient();
            /*try
            {
                await client.GetStringAsync("http://172.20.10.12/door/open");
            }
            catch(HttpRequestException ex)
            {
                throw ex;
            }*/
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

        #region WEB_API

        #endregion

        #region BDD
        
        private void UpdateTestData()
        {
            string dbName = "TestDatabase.db";
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }
            /*using (var dbContext = new MyDbContext())
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
            }*/
        }
        
        #endregion

        #region WIFI

        private void SearchWifi()
        {
            CmdWindows w = new CmdWindows();
            string[] wifis = w.GetAllWifiName();
        }

        #endregion
    }
}