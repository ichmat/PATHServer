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

        public abstract bool TryConnectWifi(string wifi_name);
    }
}
