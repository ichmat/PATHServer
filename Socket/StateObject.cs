using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SocketLibrary
{
    internal class StateObject
    {
        public Socket socket = null;
        public const int buffersize = 8192;
        public byte[] buffer = new byte[buffersize];
        public string address = string.Empty;
    }
}
