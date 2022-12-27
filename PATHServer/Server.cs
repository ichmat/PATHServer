using Newtonsoft.Json;
using PATHServer.BDD.Models;
using PATHServer.CommandEnv;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using MQTTnet.Protocol;
using PATHServer.ArduinoAction;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore;
using PATHServer.ArduinoAction.Automatisation;
using System.Runtime.InteropServices;

namespace PATHServer
{
    public class Server
    {
        public static Server instance;

        public const int PORT = 8080;

        private MqttServer _mqttServer;

        internal ArdCom ardCom;

        private CmdEnvironnement cmdEnv;

        public delegate void ServerLog(string log);

        public event ServerLog? OnServerLog;

        private static string local_ip = "127.0.0.1";

        public Server()
        {
            IdentifyOS();
            SearchWifi();
            SetLocalIPAddress();
            ardCom = new ArdCom();
            ActionIdentifier.Init();
            DataLiveManager.Init();
            UserTemperature.Init();
        }

        private void IdentifyOS()
        {
            string os = RuntimeInformation.OSDescription.ToLower();
            if (os.Contains("windows"))
            {
                cmdEnv = new CmdWindows();
            }
            else
            {
                cmdEnv = new CmdLinux();
            }
        }

        public async Task StartTest()
        {
            await StartMQTTServerTest();
        }

        private static void SetLocalIPAddress()
        {
            List<string> allIp = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    allIp.Add(ip.ToString());
                }
            }
            if (allIp.Count == 0)
            {
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

            if(allIp.Count > 1)
            {
                Console.WriteLine("please select local IP for the server : ");
                for (int i = 0; i < allIp.Count; ++i)
                {
                    Console.WriteLine(i.ToString() + " : " + allIp[i]);
                }

                bool selected = false;

                while (!selected)
                {
                    string? input = Console.ReadLine();
                    try
                    {
                        if(input != null)
                        {
                            int val = int.Parse(input);
                            if(val >= 0 && val < allIp.Count)
                            {
                                local_ip = allIp[val];
                                selected = true;
                                return;
                            }
                        }
                    }
                    catch { }
                }
            }
            local_ip = allIp[0];
        }

        public static string GetLocalIPAddress()
        {
            return local_ip;
        }

        public bool IsValidData(InfoTypeData waiting, string actionData, out string? val)
        {
            return ArdConverter.IsCorectDataForArduino(waiting, actionData, out val);
        }

        public void Log(string message)
        {
            OnServerLog?.Invoke(message);
        }

        public void Start()
        {
            // Vérifier si on a les crédential de connexion WIFI

            // Si oui : 
            ConnectToWifi();

            // si non : 
            SearchAndWaitingClientInfo();
        }

        public void ShutDown()
        {
            Console.WriteLine("shutting down ...");
            UserTemperature.StopThread();
            if (_mqttServer != null)
            {
                _mqttServer.Dispose();
            }
            MyDbContext myDbContext = new MyDbContext();
            myDbContext.WaitSaveChangesAsync().Wait();
            myDbContext.Dispose();
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

        #region BDD_COMMUNICATION

        public MyDbContext? _context = null;

        public MyDbContext ConnectToBdd()
        {
            _context = new MyDbContext();
            _context.Database.EnsureCreated();
            return _context;
        }

        public void DisconnectToBdd()
        {
            _context?.Dispose();
        }

        #endregion

        #region MQTT_SERVER

        private async Task StartMQTTServerTest()
        {
            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(10)
                .WithDefaultEndpointPort(1884)
                .WithDefaultEndpoint()
                .WithMaxPendingMessagesPerClient(10);

            _mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
            _mqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            _mqttServer.InterceptingPublishAsync += _mqttServer_InterceptingPublishAsync;
            _mqttServer.ClientDisconnectedAsync += _mqttServer_ClientDisconnectedAsync;
            await _mqttServer.StartAsync();
            OnServerLog?.Invoke("server started : " + GetLocalIPAddress());
        }

        private Task _mqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            if (ardCom.Deconnexion(arg.ClientId))
            {
                OnServerLog?.Invoke("Disconnected client");
            }
            else
            {
                OnServerLog?.Invoke("Disconnected client without any validation");
            }
            return Task.CompletedTask;
        }

        public async Task TestSendMessage()
        {
            IList<MqttClientStatus> clients = await _mqttServer.GetClientsAsync();
            var message = new MqttApplicationMessage();
            message.Topic = "ping";
            message.Payload = System.Text.UTF8Encoding.UTF8.GetBytes("ping");
            foreach(var client in clients)
            {
                await client.Session.EnqueueApplicationMessageAsync(message);
            }
        }

        public async Task<bool> SendMessage(string clientID, string topic, string message)
        {
            IList<MqttClientStatus> clients = await _mqttServer.GetClientsAsync();
            var MQTT_message = new MqttApplicationMessage();
            MQTT_message.Topic = topic;
            MQTT_message.Payload = System.Text.UTF8Encoding.UTF8.GetBytes(message);

            foreach (var client in clients)
            {
                if(client.Id == clientID) {
                    await client.Session.EnqueueApplicationMessageAsync(MQTT_message);
                    return true;
                }
            }

            return false;
        }

        public async Task SendBroadcast(string topic, string message)
        {
            IList<MqttClientStatus> clients = await _mqttServer.GetClientsAsync();
            var MQTT_message = new MqttApplicationMessage();
            MQTT_message.Topic = topic;
            MQTT_message.Payload = System.Text.UTF8Encoding.UTF8.GetBytes(message);

            foreach (var client in clients)
            {
                await client.Session.EnqueueApplicationMessageAsync(MQTT_message);
            }
        }

        private Task _mqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            ardCom.RecieveMessage(arg.ClientId,
                arg.ApplicationMessage.Topic,
                (arg.ApplicationMessage.Payload != null ?
                UTF8Encoding.UTF8.GetString(arg.ApplicationMessage.Payload) : string.Empty)
                ).Wait();
            return Task.CompletedTask;
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            if (ardCom.NewConnection(arg.ClientId))
            {
                arg.ReasonCode = MqttConnectReasonCode.Success;
                OnServerLog?.Invoke("connected client");
            }
            else
            {
                arg.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                OnServerLog?.Invoke("disconnected client");
            }

            return Task.CompletedTask;
        }

        #endregion

        #region WIFI

        private void SearchWifi()
        {
            if (cmdEnv is CmdLinux)
            {
                string[] interfaces = cmdEnv.GetWIFIInterfaces();
                ConsoleLog("current interfaces", interfaces);

                string selectedInterface = interfaces.First();
                Console.WriteLine("actual interface : " + selectedInterface);

                KNOWN_WIFI[] _WIFIs = cmdEnv.GetAllKnownWifi();
                ConsoleLog(_WIFIs);
                bool isConnectedWifi = _WIFIs.Any(x => x.connected && x.@interface! == selectedInterface);
                if (isConnectedWifi)
                {
                    KNOWN_WIFI connectedWifi = _WIFIs.First(x => x.connected && x.@interface! == selectedInterface);
                    Console.WriteLine("already connected to : " + connectedWifi.ssid + ", try disconnect ...");
                    if (cmdEnv.TryDisconnectWifi(selectedInterface, connectedWifi.ssid))
                    {
                        Console.WriteLine("disconnected");
                    }
                    else
                    {
                        Console.WriteLine("fail disconnected");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("no wifi connected");
                }

                string[] wifis = cmdEnv.GetAllWifiName();
                ConsoleLog("current wifis", wifis);

                foreach(string wifi in wifis)
                {
                    if(_WIFIs.Any(x => x.ssid == wifi))
                    {
                        Console.WriteLine("known wifi found : " + wifi + ", try connect ...");
                        if(cmdEnv.TryConnectWifi(selectedInterface, wifi))
                        {
                            Console.WriteLine("connected");
                        }
                        else
                        {
                            Console.WriteLine("fail connexion");
                        }

                        return;
                    }
                }

                Console.WriteLine("no known wifi detected");
            }
        }

        private void ConsoleLog(string title, string[] infos)
        {
            Console.WriteLine(title + " : ");
            Console.WriteLine("------------------");
            foreach (string i in infos)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("");
        }

        private void ConsoleLog(KNOWN_WIFI[] kNOWN_WIFIs)
        {
            Console.WriteLine("khown wifis : ");
            Console.WriteLine("------------------");
            foreach (KNOWN_WIFI wifi in kNOWN_WIFIs)
            {
                Console.WriteLine("ssid : " + wifi.ssid + ", connected : " + wifi.connected + (wifi.connected ? ", interface : " + wifi.@interface : string.Empty));
            }
            Console.WriteLine("");
        }

        #endregion
    }
}