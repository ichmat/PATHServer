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

        private static CultureInfo ci = CultureInfo.InvariantCulture;

        internal static bool TryGetTypeFromString(string typeName, out InfoTypeData? type)
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

            }

            type = null;
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
                        value = double.Parse(content,ci);
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
    }
}
