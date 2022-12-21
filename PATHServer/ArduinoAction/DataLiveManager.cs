using PATHServer.BDD.Models;
using PATHServer.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction
{
    public static class DataLiveManager
    {
        private static readonly DataLiveLoad[] dataLives = new DataLiveLoad[]
        {
            new DataLiveLoad("tempUser", InfoTypeData.Double),
            new DataLiveLoad("ifNightCloseAllWindow", InfoTypeData.Boolean),
            new DataLiveLoad("ifRainingCloseAllWindow", InfoTypeData.Boolean),
            new DataLiveLoad("colorLight", InfoTypeData.Rbg)
        };

        public static void Init()
        {
            dataLives.DefaultIfEmpty(DataLiveLoad.Empty);
        }

        public static bool IsExist(string liveName)
        {
            return dataLives.FirstOrDefault(x => x.Name == liveName) != DataLiveLoad.Empty;
        }

        private static bool TryGetDataLive(string liveName, out DataLiveLoad dataLive)
        {
            dataLive = dataLives.FirstOrDefault(x => x.Name == liveName);
            return dataLive != DataLiveLoad.Empty;
        }

        public static async Task<string> TryPublish(MyDbContext _context, string dataLiveName, string content)
        {
            if (TryGetDataLive(dataLiveName, out DataLiveLoad dataLive))
            {
                InfoTypeData type = dataLive.Type;
                if (!ArdConverter.IsValidData(type, content))
                    return "data invalid type";

                if (!dataLive.IsChecked)
                {
                    await CheckDataLive(_context, dataLive);
                    TryGetDataLive(dataLiveName, out dataLive);
                }

                DataLive live = _context.DataLives.First(x => x.dl_id == dataLive.dl_id);
                ArdConverter.TryConvertData(type, content, out object? value);
                if (value is double dou)
                    live.dl_val_double = dou;
                else if (value is DateTime dt)
                    live.dl_val_datetime = dt;
                else if (value is int i)
                    live.dl_val_int = i;
                else if (value is bool b)
                    live.dl_val_bool = b;

                triggerPublishedData(live.dl_name, value);
                return string.Empty;
            }
            else
            {
                return "data name not found";
            }
        }

        private static async Task CheckDataLive(MyDbContext _context, DataLiveLoad dataLive)
        {
            if(_context.DataLives.FirstOrDefault(x => x.dl_name == dataLive.Name) == null)
            {
                DataLive dl = new DataLive();
                dl.dl_name = dataLive.Name;
                _context.DataLives.Add(dl);
                await _context.WaitSaveChangesAsync();
            }

            dataLive.IsChecked = true;
            dataLive.dl_id = _context.DataLives.First(x => x.dl_name == dataLive.Name).dl_id;

            for (int i = 0; i < dataLives.Length; i++)
            {
                if(dataLives[i].Name == dataLive.Name)
                {
                    dataLives[i] = dataLive;
                    break;
                }
            }
        }

        private static void triggerPublishedData(string liveName, object? value)
        {
            OnPublishedData?.Invoke(liveName, value);
        }

        public delegate void PublishedData(string liveName, object? value);

        public static event PublishedData? OnPublishedData;
    }

    internal struct DataLiveLoad
    {
        internal static DataLiveLoad Empty = new DataLiveLoad(string.Empty, InfoTypeData.Null);

        public readonly string Name;
        public readonly InfoTypeData Type;
        public bool IsChecked;
        public int dl_id;

        internal DataLiveLoad(string name, InfoTypeData type)
        {
            this.Name = name;
            this.Type = type;
            this.IsChecked = false;
            this.dl_id = -1;
        }

        public bool Equals(DataLiveLoad obj) => this.Name == obj.Name && this.Type == obj.Type;

        public static bool operator ==(DataLiveLoad s1, DataLiveLoad s2) => (s1.Equals(s2));
        public static bool operator !=(DataLiveLoad s1, DataLiveLoad s2) => !(s1.Equals(s2));

        public override bool Equals(object? obj)
        {
            if(obj is DataLiveLoad dll) 
                return Equals(dll); 
            return false;
        }
    }
}
