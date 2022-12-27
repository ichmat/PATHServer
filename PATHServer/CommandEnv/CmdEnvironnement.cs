using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.CommandEnv
{
    public abstract class CmdEnvironnement
    {
        public abstract string[] GetAllWifiName();

        public abstract string[] GetAllKhowWifi();

        public abstract string? GetConnectedWifi(string @interface);

        public abstract bool TryConnectWifi(string @interface, string wifi_name);

        public abstract bool TryDisconnectWifi(string @interface);

        public abstract string[] GetWIFIInterfaces();

        public abstract bool CreateWIFIDirect(string wifi_interface, string wifi_name, string? mask = null);

        public delegate void DetectedConnexion(string id);

        public abstract event DetectedConnexion? OnDetectedConnexion;
    }
}
