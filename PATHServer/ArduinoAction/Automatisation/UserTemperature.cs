using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction.Automatisation
{
    internal static class UserTemperature
    {
        private const string DATA_LIVE_NAME = "tempUser";

        private static Thread thread = new Thread(ThreadRun);
         
        private static double? last_temperature;

        internal static void Init()
        {
            DataLiveManager.OnPublishedData += DataLiveManager_OnPublishedData;
        }

        private static void DataLiveManager_OnPublishedData(string liveName, object? value)
        {

        }

        private static void ThreadRun()
        {

        }

        private static void FanState(bool IsOn)
        {

        }

        private static void HeatingState(bool IsOn)
        {

        }
    }
}
