using Microsoft.EntityFrameworkCore;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PATHServer.ArduinoAction.Automatisation
{
    internal static class UserTemperature
    {
        private const string DATA_LIVE_NAME = "tempUser";
        private const string NODE_NAME = "temperatureIN";

        private const string HEATING_ACTION = "heating";
        private const string FAN_ACTION = "fan";

        private const double DELTA_TEMPERATURE = 1.0;

        private static Thread thread = new Thread(ThreadRun);

        private static SemaphoreSlim semaphoreTemperature = new SemaphoreSlim(1,1);

        private static int? _node_id_temperature;
        private static double? _last_temperature;
        private static bool _stop_pending = false;
        private static UserTempThreadState _state = UserTempThreadState.Null;

        private static bool? _fanIsOn;
        private static bool? _HeatingIsOn;

        internal static void Init()
        {
            DataLiveManager.OnPublishedData += DataLiveManager_OnPublishedData;
            StartThread();
        }

        internal static void StartThread()
        {
            _stop_pending = false;
            _fanIsOn = null;
            _HeatingIsOn = null;
            _state = UserTempThreadState.NotStarted;
            thread.Start();
        }
        internal static void StopThread()
        {
            _stop_pending = true;
        }

        private static void LogIfDebug(string log)
        {
#if DEBUG
            Server.instance.Log(log);
#endif
        }

        private static void DataLiveManager_OnPublishedData(string liveName, object? value)
        {
            if(DATA_LIVE_NAME == liveName)
            {
                semaphoreTemperature.Wait();
                if (value != null && value is double newTemp)
                {
                    _last_temperature = newTemp;
                }
                else
                {
                    _last_temperature = null;
                }
                semaphoreTemperature.Release();
            }
        }

        private static void ThreadRun()
        {
            while (!_stop_pending)
            {
                Thread.Sleep(1000);
                switch(_state)
                {
                    case UserTempThreadState.NotStarted:
                        if (_last_temperature != null || TryGetLastTemperatureFromUser())
                        {
                            _state = UserTempThreadState.Started;
                            LogIfDebug("UserTemperature : started");
                        }
                        else
                        {
                            _state = UserTempThreadState.TempNotLoaded;
                        }
                        break;
                    case UserTempThreadState.TempNotLoaded:
                        if(_last_temperature != null)
                        {
                            LogIfDebug("UserTemperature : temperature set");
                            _state = UserTempThreadState.Started;
                        }
                        break;
                    case UserTempThreadState.Started:
                        if (_last_temperature != null)
                        {
                            if (Server.instance.ardCom.HaveConnectedUser())
                            {
                                Thread_check();
                            }
                        }
                        else
                        {
                            FanState(false);
                            HeatingState(false);
                            _state = UserTempThreadState.TempNotLoaded;
                            LogIfDebug("UserTemperature : temperature unset");
                        }
                        break;
                }
            }
        }

        private static void Thread_check()
        {
            if (semaphoreTemperature.Wait(500) && _last_temperature != null && TryGetHomeTemperature(out double temperature))
            {
                double deltamin = _last_temperature!.Value - DELTA_TEMPERATURE;
                double deltamax = _last_temperature!.Value + DELTA_TEMPERATURE;

                //LogIfDebug("actual temperature : " + temperature.ToString() + " user want : " + _last_temperature.ToString()); 

                if (temperature < deltamin)
                {
                    FanState(false);
                    HeatingState(true);
                }
                else if (temperature > deltamax)
                {
                    FanState(true);
                    HeatingState(false);
                }
                else
                {
                    FanState(false);
                    HeatingState(false);
                }

                semaphoreTemperature.Release();
            }
        }

        private static bool TryGetLastTemperatureFromUser()
        {
            using (var context = new MyDbContext())
            {
                DataLive? dataLive = context.DataLives.FirstOrDefault(x => x.dl_name== DATA_LIVE_NAME);
                if(dataLive != null && dataLive.dl_val_double != null)
                {
                    _last_temperature = dataLive.dl_val_double;
                    return true;
                }
                else
                {
                    LogIfDebug("UserTemperature : no temperature setted from datalive");
                    return false;
                }
            }
        }

        private static bool TryGetHomeTemperature(out double temperature)
        {
            using (var context = new MyDbContext())
            {
                if(_node_id_temperature == null)
                {
                    Node? node = context.Nodes.FirstOrDefault(x => x.node_name == NODE_NAME);
                    if(node != null)
                    {
                        _node_id_temperature = node.node_id;
                    }
                    else
                    {
                        temperature = 0;
                        return false;
                    }
                }

                DataHistory? data = context.DataHistories.Where(x => x.node_id == _node_id_temperature).OrderByDescending(x => x.dh_date).FirstOrDefault();
                if (data != null && data is DataHistoryDouble dataHistoryDouble)
                {
                    temperature = dataHistoryDouble.dh_double_value;
                    return true;
                }
                else
                {
                    temperature = 0;
                    return false;
                }
            }
        }

        private static void FanState(bool IsOn)
        {
            if(_fanIsOn != IsOn)
            {
                Server.instance.SendBroadcast(FAN_ACTION, (IsOn ? "1" : "0")).Wait();
                LogIfDebug("UserTemperature : Fan " + (IsOn ? "On" : "Off"));
                _fanIsOn = IsOn;
            }
        }

        private static void HeatingState(bool IsOn)
        {
            if(_HeatingIsOn != IsOn)
            {
                Server.instance.SendBroadcast(HEATING_ACTION, (IsOn ? "1" : "0")).Wait();
                LogIfDebug("UserTemperature : Heating " + (IsOn ? "On" : "Off"));
                _HeatingIsOn = IsOn;
            }
        }
    }

    internal enum UserTempThreadState
    {
        Null = -1,
        NotStarted = 0,
        TempNotLoaded = 1,
        Started = 2
    }
}
