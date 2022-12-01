using SocketLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLibrary
{
    public class ServerSocket
    {
        private readonly Socket server;
        private ConcurrentDictionary<string, Socket> clients = new ConcurrentDictionary<string, Socket>();
        private ConcurrentDictionary<string, List<byte>> dataclients = new ConcurrentDictionary<string,List<byte>>();
        private ConcurrentDictionary<string, Socket> waitingclients = new ConcurrentDictionary<string, Socket>();

        private ManualResetEvent allDone = new ManualResetEvent(true);

        /// <summary>
        /// Le serveur est lancé et sur écoute
        /// </summary>
        public Action ServerStarted;
        /// <summary>
        /// Renvoie l'addresse du client detecté 
        /// </summary>
        public Action<string> DetectedClient;
        /// <summary>
        /// Renvoie l'addresse du client accepté 
        /// </summary>
        public Action<string> AcceptedClient;
        /// <summary>
        /// Renvoie le nom et l'address du client connecté
        /// </summary>
        public Action<string, string> RecieveNameClient;
        /// <summary>
        /// Renvoie l'addresse et le contenu du client 
        /// </summary>
        public Action<string, string> RecieveData;
        /// <summary>
        /// Renvoie l'addresse du client déconnecté
        /// </summary>
        public Action<string> ClientDisconnected;
        /// <summary>
        /// Renvoie une erreur
        /// </summary>
        public Action<string> ErrorLog;

        private void AcceptClient(IAsyncResult ar)
        {
            Socket server = (Socket)ar.AsyncState;
            Socket listener = server.EndAccept(ar);
            string address = listener.RemoteEndPoint.ToString();


            if (!clients.ContainsKey(address))
            {
                clients.TryAdd(address, listener);
                dataclients.TryAdd(address, new List<byte>());

                StateObject state = new StateObject();
                state.socket = listener;
                state.address = address;
                listener.BeginReceive(state.buffer, 0, StateObject.buffersize, SocketFlags.None, new AsyncCallback(receiveMessage), state);

                AcceptedClient?.Invoke(address);
            }

            server.BeginAccept(new AsyncCallback(AcceptClient), (Socket)server);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSocket"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public ServerSocket(int port)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            server.Bind(ip);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            server.Listen(20);
            ServerStarted?.Invoke();
            server.BeginAccept(new AsyncCallback(AcceptClient), (Socket)server);
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        /// <param name="address">The client address.</param>
        public void DisconnectClient(string address)
        {
            if (clients.ContainsKey(address))
            {
                try
                {
                    Socket socket = clients[address];
                    SendDisconnected(address);
                    clients.TryRemove(address, out Socket s);
                    dataclients.TryRemove(address, out List<byte> val);
                    socket.Disconnect(false);
                    ClientDisconnected?.Invoke(address);
                }
                catch (Exception e)
                {
                    ErrorLog?.Invoke(e.ToString());
                }
            }
        }

        /// <summary>
        /// Sends the specified message to the client.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The name sender.</param>
        public void Send(string address,string message,string sender = "server")
        {
            try
            {
                Socket socket;
                if (clients.TryGetValue(address, out socket))
                {
                    byte[] datasSending = new MessageSocket(sender, DateTime.Now, message).ToByte();
                    if (MessageSocket.IsMessageLenghtNeeded(datasSending))
                    {
                        socket.Send(MessageSocket.CreateLengthMessage(sender, datasSending.Length));
                    }
                    socket.Send(datasSending);
                }
            }
            catch (Exception e){
                ErrorLog?.Invoke(e.ToString());
            }
        }

        /// <summary>
        /// Sends message to all clients connected.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="exception">The client exception.</param>
        public void SendBroadcast(string message, string sender = "server", string exception = null)
        {
            try { 
                MessageSocket ms = new MessageSocket(sender, DateTime.Now, message);
                sendBroadCastMessage(ms, exception);
            }
            catch(Exception e) {
                ErrorLog?.Invoke(e.ToString());
            }
        }

        /// <summary>
        /// Sends the validation message to the client.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="sender">The sender.</param>
        public void SendValidation(string address, string sender = "server")
        {
            try
            {
                Socket socket;
                if (clients.TryGetValue(address, out socket))
                {
                    socket.Send(MessageSocket.CreateValidationMessage(sender));
                }
                else
                {
                    ErrorLog?.Invoke("SendValidation : client not found");
                }
            }
            catch (Exception e)
            {
                ErrorLog?.Invoke(e.ToString());
            }
        }

        #region TIMER

        private readonly ConcurrentDictionary<string, System.Timers.Timer> timeout = new ConcurrentDictionary<string, System.Timers.Timer>();
        private static int TIMEOUT = 5000;

        private void Timeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string address;

            System.Timers.Timer timer = (System.Timers.Timer)sender;
            address = timeout.FirstOrDefault(x => x.Value == timer).Key;

            dataclients[address].Clear();
            StopTimer(address);
            ErrorLog?.Invoke("/!\\ Time out for " + address);
        }

        private void ReloadTimer(string address)
        {
            StopTimer(address);
            StartTimer(address);
        }

        private void StartTimer(string address)
        {
            try
            {
                timeout.TryAdd(address, new System.Timers.Timer(TIMEOUT));
                timeout[address].AutoReset = false;
                timeout[address].Elapsed += Timeout_Elapsed;
                timeout[address].Start();
            }
            catch { }
        }

        private void StopTimer(string address)
        {
            if (timeout.ContainsKey(address))
            {
                timeout[address].Stop();
                timeout[address].Elapsed -= Timeout_Elapsed;
                timeout.TryRemove(address, out System.Timers.Timer t);
            }
        }

        #endregion

        #region MESSAGE

        private void receiveMessage(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            
            try
            {
                Socket handler = state.socket;
                if (!handler.Connected)
                {
                    clients.TryRemove(state.address, out Socket s);
                    dataclients.TryRemove(state.address, out List<byte> l);
                    ClientDisconnected?.Invoke(state.address);
                }
                if (!dataclients.ContainsKey(state.address))
                    return;

                int i = handler.EndReceive(ar);
                dataclients[state.address].AddRange(state.buffer.Take(i));
                if (MessageSocket.IsMessageSocket(dataclients[state.address]))
                    messageAction(in state);
                else
                    ReloadTimer(state.address);

                state.socket.BeginReceive(state.buffer, 0, StateObject.buffersize, SocketFlags.None, new AsyncCallback(receiveMessage), state);
            }
            catch(Exception)
            {
                try
                {
                    //ErrorLog?.Invoke(ex.Message + Environment.NewLine + ex.StackTrace);
                    clients.TryRemove(state.address, out Socket s);
                    ClientDisconnected?.Invoke(state.address);
                }
                catch { }
            }
        }

        private void messageAction(in StateObject state)
        {
            StopTimer(state.address);

            MessageSocket ms = MessageSocket.TryCreate(dataclients[state.address].ToArray());
            
            if (MessageSocket.IsRequestName(ms))
                RecieveNameClient?.Invoke(ms.Sender, state.address);
            else
                RecieveData?.Invoke(state.address,ms.Content);

            dataclients[state.address].Clear();
        }

        private void sendBroadCastMessage(MessageSocket data, string exception = null)
        {
            Socket sExc = null;
            if (exception != null)
                clients.TryGetValue(exception, out sExc);

            byte[] datasSending = data.ToByte();
            byte[] beforeSending = null;
            if (MessageSocket.IsMessageLenghtNeeded(datasSending))
            {
                beforeSending = MessageSocket.CreateLengthMessage(data.Sender, datasSending.Length);
            }
            foreach (Socket socket in clients.Values)
            {

                if ((sExc != null && sExc != socket) || sExc == null)
                {
                    if (beforeSending != null) socket.Send(beforeSending);
                    socket.Send(datasSending);
                }
            }
        }

        private void SendDisconnected(string address, string sender = "server")
        {
            Socket socket;
            if (clients.TryGetValue(address, out socket))
            {
                socket.Send(MessageSocket.CreateDisconnectMessage(sender));
            }
        }

        #endregion
    }
}
