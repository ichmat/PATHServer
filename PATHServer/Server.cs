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

namespace PATHServer
{
    public class Server
    {
        public static Server instance;

        public const int PORT = 8080;

        private MqttServer _mqttServer;

        private ArdCom _ardCom;

        public delegate void ServerLog(string log);

        public event ServerLog? OnServerLog;

        public Server()
        {
            _ardCom = new ArdCom();
        }
#if DEBUG
        public async Task StartTest()
        {
            await StartMQTTServerTest();
        }

#endif

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
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884)
                .WithDefaultEndpoint();

            _mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
            _mqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            _mqttServer.InterceptingPublishAsync += _mqttServer_InterceptingPublishAsync;
            _mqttServer.ClientDisconnectedAsync += _mqttServer_ClientDisconnectedAsync;
            await _mqttServer.StartAsync();
            OnServerLog?.Invoke("server started");
        }

        private Task _mqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            if (_ardCom.Deconnexion(arg.ClientId))
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

        private async Task _mqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            await _ardCom.RecieveMessage(arg.ClientId,
                arg.ApplicationMessage.Topic,
                UTF8Encoding.UTF8.GetString(arg.ApplicationMessage.Payload));
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            if (_ardCom.NewConnection(arg.ClientId))
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
            CmdWindows w = new CmdWindows();
            string[] wifis = w.GetAllWifiName();
        }

        #endregion
    }
}