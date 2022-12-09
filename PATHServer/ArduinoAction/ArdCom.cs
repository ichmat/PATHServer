using Microsoft.EntityFrameworkCore;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction
{
    public class ArdCom
    {
        private readonly Dictionary<string, bool> validated;

        private const string TOPIC_INIT = "init";
        private const string TOPIC_VALIDATE = "validate";

        public ArdCom() 
        {
            validated = new Dictionary<string, bool>();
        }

        public bool NewConnection(string userID)
        {
            if(string.IsNullOrWhiteSpace(userID) || validated.ContainsKey(userID)) { return false; }
            validated.Add(userID, false);
            return true;
        }

        public bool Deconnexion(string userID)
        {
           if(validated.ContainsKey(userID))
           {
                validated.Remove(userID);
                return true;
           }
            return false;

        }

        private bool CanInteract(string userID)
        {
            return validated.ContainsKey(userID) && validated[userID];
        }

        private void Log(string message)
        {
            Server.instance.Log(message);
        }

        public async Task<bool> RecieveMessage(string userID, string topic, string message)
        {
            //if (!CanInteract(userID)) return false;
            MyDbContext dbContext = Server.instance.ConnectToBdd();
            switch (topic)
            {
                case TOPIC_INIT:
                    await Init_Nodes(dbContext, message); break;
                default:
                    break;
            }
            return true;
        }

        #region MESSAGE_ACTION

        private const char SEPARATOR_INIT_NODE = ';';
        private const char SEPARATOR_INIT_VALUE = ',';

        private async Task NodeInfo(MyDbContext dbContext, string topic, string message)
        {
            if(!string.IsNullOrWhiteSpace(topic))
            {
                Node? find = await dbContext.Nodes.FirstOrDefaultAsync(x => x.node_name == topic);
                if(find!= null)
                {
                    if(ArdConverter.TryConvertData(find.NodeTypeData, message, out object? value))
                    {

                    }
                    else
                    {
                        Log("NodeInfo : ⚠ node incorrect information of '" + topic + "' message is '" + message + "', type needed is : " + find.NodeTypeData.ToString());
                    }
                }
                else
                {
                    Log("NodeInfo : ⚠ node name : '" + topic + "' not found");
                }
            }
            else
            {
                Log("NodeInfo : ⚠ node name empty");
            }
        }

        private async Task Init_Nodes(MyDbContext dbContext, string message)
        {
            string[] allNodes = message.Split(SEPARATOR_INIT_NODE);
            bool nodeCreated = false;
            Log("--- Start Init ---");
            Log("Nb nodes send : " + allNodes.Length);
            foreach (string nodeInfo in allNodes)
            {
                string[] arrInfos = nodeInfo.Split(SEPARATOR_INIT_VALUE);
                if(arrInfos.Length == 2)
                {
                    string nodeName = arrInfos[0];
                    string dataType = arrInfos[1];
                    if(!string.IsNullOrWhiteSpace(nodeName) && 
                        ArdConverter.TryGetTypeFromString(dataType, out InfoTypeData? typeData))
                    {
                        Node? find = await dbContext.Nodes.FirstOrDefaultAsync(x => x.node_name == nodeName);
                        if(find == null)
                        {
                            Node n = new Node();
                            n.node_name = nodeName;
                            n.node_type_data = (int)typeData!;
                            nodeCreated = true;
                            await dbContext.Nodes.AddAsync(n);
                        }
                    }
                    else
                    {
                        Log("⚠ not correct info, 'nodeName' : " + nodeName + ", 'dataType' : " + dataType);
                    }
                }
                else
                {
                    Log("⚠ incorrect data size, actual : " + arrInfos.Length + ", need : 2");
                }
            }

            if(!nodeCreated)
                Log("Already up to date");
            else
                await dbContext.SaveChangesAsync();
            Log("--- End Init ---");
        }

        #endregion
    }
}
