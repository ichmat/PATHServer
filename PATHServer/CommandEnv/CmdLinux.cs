using CliWrap;
using CliWrap.Buffered;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.CommandEnv
{
    internal class CmdLinux : CmdEnvironnement
    {

        /*
* var result = Cli
         .Wrap("/dev/init.d/mnw")
         .WithArguments("stop")
         .ExecuteBufferedAsync();
*/

        /*
         * scan wifi : 
         * iwlist wlan0 scan
         * 
         * connect to wifi : 
         * nmcli device wifi connect SSID-NAME password YOUR-PASSWORD
         * 
         * wifi already connected : 
         * nmcli connection show
         * 
         * disconnect :
         * nmcli connection down id SSID-NAME
         */

        public override event DetectedConnexion? OnDetectedConnexion;

        public override string[] GetAllWifiName()
        {
            var result = Cli
                  .Wrap("iwlist")
                  .WithArguments("wlan0 scan")
                  .ExecuteBufferedAsync();

            result.Task.Wait();

            string[] outputs = result.Task.Result.StandardOutput.Split(Environment.NewLine);
            //Console.WriteLine("number of lines : " + outputs.Length);

            List<string> allWifis = new List<string>();

            for(int i = 0; i < outputs.Length; ++i)
            {
                if (outputs[i].Contains("ESSID"))
                {
                    string essid = outputs[i].Split(':')[1].Trim(' ','"');
                    //Console.WriteLine("found : " + essid);
                    allWifis.Add(essid);
                }
            }

            return allWifis.ToArray();
        }

        public override string? GetConnectedWifi(string @interface)
        {
            var result = Cli
                 .Wrap("nmcli")
                 .WithArguments("connection show")
                 .ExecuteBufferedAsync();

            result.Task.Wait();

            string[] outputs = result.Task.Result.StandardOutput.Split(Environment.NewLine);

            int i_name_start = -1;
            int i_name_end = -1;

            string? essid = null;

            foreach (string line in outputs)
            {
                if (line.Contains("NAME") && line.Contains("UUID") && line.Contains("TYPE") && line.Contains("DEVICE"))
                {
                    i_name_start = line.IndexOf("NAME");
                    i_name_end = line.IndexOf("UUID") - 1;
                }
                else
                {
                    if (line.Contains(@interface))
                    {
                        return line.Skip(i_name_start).Take(i_name_end - i_name_start).ToString();
                        break;
                    }
                }
            }

            return null;
        }

        public override bool TryConnectWifi(string @interface, string wifi_name)
        {
            var result = Cli
                  .Wrap("sudo")
                  .WithArguments("nmcli device wifi connect \"" + wifi_name + '"')
                  .ExecuteBufferedAsync();

            result.Task.Wait();
            string output = result.Task.Result.StandardOutput;
            return output.Contains("successfully activated");
        }

        public override bool TryDisconnectWifi(string @interface)
        {
            string? essid = this.GetConnectedWifi(@interface);

            if (essid != null)
            {
                var result = Cli
                 .Wrap("sudo")
                 .WithArguments("nmcli connection down id \"" + essid + '"')
                 .ExecuteBufferedAsync();
                result.Task.Wait();
                return result.Task.Result.StandardOutput.Contains("successfully deactivated");
            }

            return false;
        }

        public override string[] GetWIFIInterfaces()
        {
            var result = Cli
                  .Wrap("iw")
                  .WithArguments("dev")
                  .ExecuteBufferedAsync();

            result.Task.Wait();

            string[] outputs = result.Task.Result.StandardOutput.Split(Environment.NewLine);

            List<string> allInterfaces = new List<string>();

            for (int i = 0; i < outputs.Length; ++i)
            {
                if (outputs[i].Contains("Interface"))
                {
                    string @interface = outputs[i].Split(' ')[1].Trim(' ', '"');
                    //Console.WriteLine("found : " + essid);
                    allInterfaces.Add(@interface);
                }
            }

            return allInterfaces.ToArray();
        }

        public override bool CreateWIFIDirect(string wifi_interface, string wifi_name, string? mask = null)
        {
            throw new NotImplementedException();
        }

       
    }
}
