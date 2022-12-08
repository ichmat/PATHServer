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

namespace PATHServer
{
    public class Server
    {
        public const int PORT = 8080;

        private IManagedMqttClient _mqttClient;
        private MqttServer _mqttServer;

        public delegate void ServerLog(string log);

        public event ServerLog? OnServerLog;

        public Server()
        {
            
        }
#if DEBUG
        public async Task StartTest()
        {
            await StartMQTTServerTest();
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

        #region MQTT

        #region SERVER

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
            OnServerLog?.Invoke("Disconnected client");
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

        private Task _mqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            OnServerLog?.Invoke("topic : " + arg.ApplicationMessage.Topic + " message " + System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload));
            return Task.CompletedTask;
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            arg.ReasonCode = MqttConnectReasonCode.Success;
            OnServerLog?.Invoke("connected client");
            return Task.CompletedTask;
        }

        #endregion

        private async Task StartMQTTClientTest()
        {
            // Creates a new client
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                                    .WithClientId("PATHServer")
                                                    .WithTcpServer("localhost", 707);

            // Create client options objects
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            // Creates the client object
            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            // Set up handlers
            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync;
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;
            _mqttClient.ConnectingFailedAsync += _mqttClient_ConnectingFailedAsync;

            // Starts a connection with the Broker
            await _mqttClient.StartAsync(options);
            // Send a new message to the broker every second
            string json = JsonConvert.SerializeObject(new { message = "Heyo :)", sent = DateTimeOffset.UtcNow });
            await _mqttClient.EnqueueAsync("dev.to/topic/json", json);
        }

        private Task _mqttClient_ConnectingFailedAsync(ConnectingFailedEventArgs arg)
        {
            OnServerLog?.Invoke("MQTT Client ConnectingFailed : " + arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            OnServerLog?.Invoke("MQTT Client Disconnected");
            return Task.CompletedTask;
        }

        private Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            OnServerLog?.Invoke("MQTT Client Connected");
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