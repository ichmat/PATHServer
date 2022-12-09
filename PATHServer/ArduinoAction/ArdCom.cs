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

        private readonly Dictionary<string, NodeTypeData> actions;

        public ArdCom() 
        {
            validated = new Dictionary<string, bool>();
            actions = new Dictionary<string, NodeTypeData>();
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
                    await InitNodes(dbContext, message); 
                    break;
                default:
                    await NodeInfo(dbContext, topic, message);
                    break;
            }
            return true;
        }

        #region MESSAGE_ACTION

        private const char SEPARATOR_INIT_NODE = ';';
        private const char SEPARATOR_INIT_VALUE = ':';

        private async Task NodeInfo(MyDbContext dbContext, string topic, string message)
        {
            if(!string.IsNullOrWhiteSpace(topic))
            {
                Node? find = await dbContext.Nodes.FirstOrDefaultAsync(x => x.node_name == topic);
                if(find!= null)
                {
                    if(ArdConverter.TryConvertData(find.NodeTypeData, message, out object? value))
                    {
                        await CreateHistory(dbContext, find!, value!);
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

        private T CreateModel<T>(Node node) where T : DataHistory
        {
            T dh = (T)Activator.CreateInstance(typeof(T))!;
            dh.dh_date = DateTime.Now;
            dh.node_id = node.node_id;
            return dh;
        }

        private async Task CreateHistory(MyDbContext dbContext, Node node, object data)
        {
            if(data is string str)
            {
                DataHistoryString dhs = CreateModel<DataHistoryString>(node);
                dhs.dh_string_value = str;
                await dbContext.DataHistoryStrings.AddAsync(dhs);
            }
            else if(data is DateTime dt)
            {
                DataHistoryDate dhd = CreateModel<DataHistoryDate>(node);
                dhd.dh_date_value = dt;
                await dbContext.DataHistoryDates.AddAsync(dhd);
            }
            else if (data is double db)
            {
                DataHistoryDouble dhd = CreateModel<DataHistoryDouble>(node);
                dhd.dh_double_value = db;
                await dbContext.DataHistoryDoubles.AddAsync(dhd);
            }
            else if (data is int i)
            {
                DataHistoryInt dhi = CreateModel<DataHistoryInt>(node);
                dhi.dh_int_value = i;
                await dbContext.DataHistoryInts.AddAsync(dhi);
            }
            else if (data is bool b)
            {
                DataHistoryBool dhb = CreateModel<DataHistoryBool>(node);
                dhb.dh_bool_value = b;
                await dbContext.DataHistoryBools.AddAsync(dhb);
            }
            else
            {
                Log("⚠ data not created, unsuported type");
            }
        }

        private async Task InitNodes(MyDbContext dbContext, string message)
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
                    if(!string.IsNullOrWhiteSpace(nodeName))
                    {
                        if (ArdConverter.IsAction(dataType, out NodeTypeData? type))
                        {
                            if (!actions.ContainsKey(nodeName))
                            {
                                actions.Add(nodeName, type!.Value);
                            }
                            else
                            {
                                //Log("⚠ action already exist, 'nodeName' : " + nodeName);
                            }
                        }
                        else if(ArdConverter.IsTypeData(dataType, out NodeTypeData? typeData) &&
                        typeData != NodeTypeData.Rbg)
                        {
                            Node? find = await dbContext.Nodes.FirstOrDefaultAsync(x => x.node_name == nodeName);
                            if (find == null)
                            {
                                Node n = new Node();
                                n.node_name = nodeName;
                                n.node_type_data = (int)typeData!;
                                nodeCreated = true;
                                await dbContext.Nodes.AddAsync(n);
                            }
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
