using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction
{
    internal static class ArdConverter
    {
        private const string INT = "int";
        private const string LONG = "long";
        private const string FLOAT = "float";
        private const string DOUBLE = "double";
        private const string STRING = "string";
        private const string BOOLEAN = "boolean";
        private const string RGB = "rgb";

        private const char SEPARATOR = '_';
        private const string ACTION = "action";

        private const string EVENT = "event";


        private static CultureInfo ci = CultureInfo.InvariantCulture;

        internal static bool IsTypeData(string typeName, out InfoTypeData? type)
        {
            typeName = typeName.Trim().ToLower();
            switch (typeName)
            {
                case INT:
                    type = InfoTypeData.Int; return true;
                case LONG:
                    type = InfoTypeData.Long; return true;
                case FLOAT:
                case DOUBLE:
                    type = InfoTypeData.Double; return true;
                case STRING:
                    type = InfoTypeData.String; return true;
                case BOOLEAN:
                    type = InfoTypeData.Boolean; return true;
                case RGB:
                    type = InfoTypeData.Rbg; return true;

            }

            type = null;
            return false;
        }

        internal static bool IsAction(string typeName, out InfoTypeData? type)
        {
            string[] actions_data = typeName.Trim().ToLower().Split(SEPARATOR);
            type = null;
            if (actions_data.Length == 2)
            {
                string action = actions_data[0];
                string typeStr = actions_data[1];
                if (action == ACTION && IsTypeData(typeStr, out type))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsEvent(string typeName, out InfoTypeData? type)
        {
            string[] event_data = typeName.Trim().ToLower().Split(SEPARATOR);
            type = null;
            if (event_data.Length == 2)
            {
                string action = event_data[0];
                string typeStr = event_data[1];
                if (action == EVENT && IsTypeData(typeStr, out type))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryConvertData(InfoTypeData type, string content, out object? value)
        {
            try
            {
                switch (type)
                {
                    case InfoTypeData.Boolean:
                        value = Convert.ToBoolean(content);
                        return true;
                    case InfoTypeData.Double:
                        value = double.Parse(content, ci);
                        return true;
                    case InfoTypeData.String:
                        value = Convert.ToString(content);
                        return true;
                    case InfoTypeData.Int:
                        value = Convert.ToInt32(content);
                        return true;
                    case InfoTypeData.Long:
                        value = Convert.ToInt64(content);
                        return true;
                    case InfoTypeData.Date:
                        value = Convert.ToDateTime(content);
                        return true;
                }
                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        internal static bool IsCorectDataForArduino(InfoTypeData type, string content, out string? value)
        {
            try
            {
                switch (type)
                {
                    case InfoTypeData.Boolean:
                        if(content == "0" || content == "1")
                        {
                            value = content;
                            return true;
                        }
                        else
                        {
                            value = null;
                            return false;
                        }
                        
                    case InfoTypeData.Double:
                        value = double.Parse(content, ci).ToString(ci);
                        return true;
                    case InfoTypeData.String:
                        value = Convert.ToString(content);
                        return true;
                    case InfoTypeData.Int:
                        value = Convert.ToInt32(content).ToString();
                        return true;
                    case InfoTypeData.Long:
                        value = Convert.ToInt64(content).ToString();
                        return true;
                    case InfoTypeData.Date:
                        value = Convert.ToDateTime(content).ToString();
                        return true;
                    case InfoTypeData.Rbg:
                        string[] arr_int = content.Split(';');
                        if(arr_int.Length == 3)
                        {
                            for(int i = 0; i < arr_int.Length; ++i) 
                            {
                                int val = Convert.ToInt32(arr_int[i]);
                                if(val < 0 || val > 255)
                                {
                                    value = null;
                                    return false;
                                }
                                while (arr_int[i].Length != 3)
                                {
                                    arr_int[i] = '0' + arr_int[i];
                                }
                            }
                            value = arr_int[0] + ';' + arr_int[1] + ';' + arr_int[2];
                            return true;
                        }
                        value = null;
                        return false;
                }
                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        internal static bool IsValidData(InfoTypeData type, string content)
        {
            try
            {
                switch (type)
                {
                    case InfoTypeData.Boolean:
                        if (content == "0" || content == "1")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case InfoTypeData.Double:
                        double.Parse(content, ci);
                        return true;
                    case InfoTypeData.String:
                        Convert.ToString(content);
                        return true;
                    case InfoTypeData.Int:
                        Convert.ToInt32(content);
                        return true;
                    case InfoTypeData.Long:
                        Convert.ToInt64(content);
                        return true;
                    case InfoTypeData.Date:
                        Convert.ToDateTime(content);
                        return true;
                    case InfoTypeData.Rbg:
                        string[] arr_int = content.Split(';');
                        if (arr_int.Length == 3)
                        {
                            foreach (string str_int in arr_int)
                            {
                                int val = Convert.ToInt32(str_int);
                                if (val < 0 && val > 255)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                        return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
