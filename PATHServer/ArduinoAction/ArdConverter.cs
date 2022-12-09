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

        internal static bool TryGetTypeFromString(string typeName, out NodeTypeData? type)
        {
            typeName = typeName.Trim().ToLower();
            switch (typeName)
            {
                case INT:
                    type = NodeTypeData.Int; return true;
                case LONG:
                    type = NodeTypeData.Long; return true;
                case FLOAT:
                case DOUBLE:
                    type = NodeTypeData.Double; return true;
                case STRING:
                    type = NodeTypeData.String; return true;
                case BOOLEAN:
                    type = NodeTypeData.Boolean; return true;

            }

            type = null;
            return false;
        }

        internal static bool TryConvertData(NodeTypeData type, string content, out object? value)
        {
            try
            {
                switch (type)
                {
                    case NodeTypeData.Boolean:
                        value = Convert.ToBoolean(content);
                        return true;
                    case NodeTypeData.Double:
                        value = double.Parse(content,ci);
                        return true;
                    case NodeTypeData.String:
                        value = Convert.ToString(content);
                        return true;
                    case NodeTypeData.Int: 
                        value = Convert.ToInt32(content);
                        return true;
                    case NodeTypeData.Long:
                        value = Convert.ToInt64(content);
                        return true;
                    case NodeTypeData.Date:
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
