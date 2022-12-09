using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        private const char ACTION_SEPARATOR = '_';
        private const string ACTION = "action";
        private const string RGB = "rgb";

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
            string[] actions_data = typeName.Trim().ToLower().Split(ACTION_SEPARATOR);
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
    }
}
