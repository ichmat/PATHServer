using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.CommandEnv
{
    public abstract class CmdEnvironnement
    {
        private const string _wifi_credential = "wcred";

        public const string SSID_Default = "PATHServer";
        public const string PWD_Default = "PATHServer";

        public abstract string[] GetAllWifiName();

        public abstract KNOWN_WIFI[] GetAllKnownWifi();

        public abstract string? GetConnectedWifi(string @interface);

        public abstract bool TryConnectWifi(string @interface, string wifi_name);

        public abstract bool TryDisconnectWifi(string @interface);

        public abstract bool TryDisconnectWifi(string @interface, string wifi_name);

        public abstract string[] GetWIFIInterfaces();

        public bool TryGetWifiCredentials(out string ssid, out string pwd)
        {
            if(File.Exists(_wifi_credential))
            {
                try
                {
                    string[] outputs = File.ReadAllLines(_wifi_credential);
                    ssid = outputs[0].Split(':')[1];
                    pwd = outputs[1].Split(':')[1];
                    return true;
                }
                catch { }
            }
            ssid = string.Empty;
            pwd = string.Empty;
            return false;
        }

        public void WriteCredentials(string ssid, string pwd)
        {
            string txt = "ssid:" + ssid + Environment.NewLine;
            txt += "pwd:"+pwd;
            File.WriteAllText(_wifi_credential, txt);
        }

        public static string NormalizeWhiteSpace(string input)
        {
            int len = input.Length,
                index = 0,
                i = 0;
            var src = input.ToCharArray();
            char ch;
            for (; i < len; i++)
            {
                ch = src[i];
                if(IsWhiteSpace(ch))
                {
                    if (len == i + 1 || IsWhiteSpace(src[i + 1])) break;
                    src[index++] = ch;
                    continue;
                }
                else
                {
                    src[index++] = ch;
                    continue;
                }
            }

            return new string(src, 0, index);
        }

        private static bool IsWhiteSpace(char ch)
        {
            switch (ch)
            {
                case '\u0020':
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                case '\u2028':
                case '\u2029':
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0085':
                    return true;
                default:
                    return false;
            }
        }
    }

    public struct KNOWN_WIFI
    {
        public readonly string ssid;
        public readonly string uuid;
        public readonly bool connected;
        public readonly string? @interface;

        public KNOWN_WIFI(string ssid, string uuid, bool connected, string? @interface)
        {
            this.ssid = ssid;
            this.uuid = uuid;
            this.connected = connected;
            this.@interface = @interface;
        }
    }
}
