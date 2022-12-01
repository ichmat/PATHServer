using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SocketLibrary
{
    public class ClientSocket
    {
        private readonly string name;
        private Socket client;
        private readonly int port;
        private readonly string ip;
        private bool isValidate = false;
        private List<byte> datarecieved = new List<byte>();
        public bool IsConnected { get; private set; } = false;

        private int sizeMessageIncoming = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSocket"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="name">The client name.</param>
        public ClientSocket(string ip,int port,string name = "<unkown>")
        {
            this.ip = ip;
            this.port = port;
            this.name = name;
        }

        /// <summary>
        /// Le client est connecté
        /// </summary>
        public Action ClientConnected;
        /// <summary>
        /// Le client est deconnecté
        /// </summary>
        public Action ClientDisconnected;
        /// <summary>
        /// La connexion avec l'hôte a été intérompue
        /// </summary>
        public Action InterruptedConnexion;
        /// <summary>
        /// La connexion à été refusé
        /// </summary>
        public Action ConnexionRefused;
        /// <summary>
        /// Le client reçois un message du server (arg1 => origine, arg2 => message)
        /// </summary>
        public Action<string,string> RecieveData;
        /// <summary>
        /// Le client reçois un message incomplet du server (arg1 => taille du message en cours, arg2 => taille du message attendu)
        /// </summary>
        public Action<int, int> IncommingData;
        /// <summary>
        /// Detection d'une erreur
        /// </summary>
        public Action<string> error;

        /// <summary>
        /// Starts the client socket.
        /// </summary>
        public void Start()
        {
            if (client != null)
                client.Dispose();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ip, port);
            IsConnected = true;
            StateObject state = new StateObject();
            state.socket = client;
            client.BeginReceive(state.buffer, 0, StateObject.buffersize, SocketFlags.None, new AsyncCallback(receiveMessage), state);
        }

        public bool TryStart()
        {
            try
            {
                if(client != null)
                    client.Dispose();
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(ip, port);
                IsConnected = true;
                StateObject state = new StateObject();
                state.socket = client;
                client.BeginReceive(state.buffer, 0, StateObject.buffersize, SocketFlags.None, new AsyncCallback(receiveMessage), state);
                return true;
            }catch(SocketException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Disconnects the client socket.
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            client.Close();
            ClientDisconnected?.Invoke();
        }

        #region TIMER

        private static int TIMEOUT = 5000;
        private Timer timeout = null;

        private void ReloadTimer()
        {
            StopTimer();
            StartTimer();
        }

        private void StartTimer()
        {
            timeout = new Timer(TIMEOUT);
            timeout.AutoReset = false;
            timeout.Elapsed += Timeout_Elapsed;
            timeout.Start();
        }

        private void StopTimer()
        {
            if (timeout != null)
            {
                timeout.Stop();
                timeout.Elapsed -= Timeout_Elapsed;
            }
        }

        private void Timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            error?.Invoke("recieve message timeout");
            datarecieved.Clear();
            StopTimer();
        }

        #endregion

        #region MESSAGE

        /// <summary>
        /// Sends the client name to the server.
        /// </summary>
        public void SendName()
        {
            client.Send(MessageSocket.CreateRequestName(name));
        }

        /// <summary>
        /// Sends the specified message to the server.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(string message)
        {
            MessageSocket ms = new MessageSocket(name, DateTime.Now, message);
            if(IsConnected)
                client.Send(ms.ToByte());
        }

        private void receiveMessage(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.socket;
            
            try
            {
                int i = handler.EndReceive(ar);
                datarecieved.AddRange(state.buffer.Take(i));
                if (MessageSocket.IsMessageSocket(in datarecieved))
                    messageAction(state);
                else
                {
                    IncommingData?.Invoke(datarecieved.Count, sizeMessageIncoming);
                    ReloadTimer();
                }

                client.BeginReceive(state.buffer, 0, StateObject.buffersize, SocketFlags.None, new AsyncCallback(receiveMessage), state);
            }
            catch (SocketException e)
            {
                error?.Invoke(e.Message);
                InterruptedConnexion?.Invoke();
                IsConnected = false;
            }
            catch (Exception e)
            {
                error?.Invoke(e.Message);
                IsConnected = false;
            }
        }

        private void messageAction(StateObject state)
        {
            StopTimer();
            MessageSocket ms = MessageSocket.TryCreate(datarecieved.ToArray());
            if (MessageSocket.IsValidationMsg(ms))
            {
                isValidate = true;
                ClientConnected?.Invoke();
            }
            else if(MessageSocket.IsDisconnectMsg(ms))
            {
                IsConnected = false;
                if (!isValidate)
                    ConnexionRefused?.Invoke();
                else
                    ClientDisconnected?.Invoke();
            }
            else if (MessageSocket.IsMessageLength(ms))
            {
                sizeMessageIncoming = MessageSocket.GetLengthMessage(ms);
            }
            else
            {
                sizeMessageIncoming = -1;
                RecieveData?.Invoke(ms.Sender,ms.Content);
            }
            datarecieved.Clear();
        }

        #endregion
    }
}
