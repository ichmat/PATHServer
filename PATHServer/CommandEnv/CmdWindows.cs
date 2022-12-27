using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.CommandEnv
{
    internal class CmdWindows : CmdEnvironnement
    {
        private const string CMD = "CMD.exe";

        private readonly List<string> datas = new List<string>();

        public override event DetectedConnexion? OnDetectedConnexion;

        public override bool CreateWIFIDirect(string wifi_interface, string wifi_name, string? mask = null)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllWifiName()
        {
            datas.Clear();
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WorkingDirectory = @"C:\";
            p.StartInfo.Verb = "runas";
            p.StartInfo.FileName = CMD;
            p.Start();
            p.StandardInput.WriteLine("netsh wlan show networks" + p.StandardInput.NewLine);
            Thread.Sleep(50);
            p.StandardInput.WriteLine("end command" + p.StandardInput.NewLine);
            Thread.Sleep(50);
            bool readNow = false;
            while (p.StandardOutput.Peek() > -1)
            {
                string cmd = p.StandardOutput.ReadLine();
                if(cmd.Contains("netsh wlan show networks"))
                {
                    readNow = true;
                    continue;
                }
                if (cmd.Contains("end command")) break;
                if(readNow)
                    datas.Add(cmd);
            }

            List<string> listWifi = new List<string>();

            foreach (string line in datas)
            {
                if (line.Contains("SSID"))
                {
                    string wifi = line.Split(':')[1].Remove(0,1);
                    if (!string.IsNullOrWhiteSpace(wifi))
                    {
                        listWifi.Add(wifi);
                    }
                }
            }

            return listWifi.ToArray();
        }

        public override string? GetConnectedWifi(string @interface)
        {
            throw new NotImplementedException();
        }

        public override string[] GetWIFIInterfaces()
        {
            throw new NotImplementedException();
        }

        public override bool TryConnectWifi(string @interface, string wifi_name)
        {
            throw new NotImplementedException();
        }

        public override bool TryDisconnectWifi(string @interface)
        {
            throw new NotImplementedException();
        }
    }
}
