using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction
{
    public class ArdCom
    {
        private readonly Dictionary<string, bool> validated;

        private const string TOPIC_INIT = "init";
        private const string TOPIC_VALIDATE = "validate";

        private readonly Dictionary<string, InfoTypeData> _event = new Dictionary<string, InfoTypeData>();

        public ArdCom()
        {
            validated = new Dictionary<string, bool>();
        }

        public bool NewConnection(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID) || validated.ContainsKey(userID)) { return false; }
            validated.Add(userID, false);
            return true;
        }

        public bool Deconnexion(string userID)
        {
            if (validated.ContainsKey(userID))
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
            if (!string.IsNullOrWhiteSpace(topic))
            {
                if (_event.ContainsKey(topic))
                {
                    // 
                    return;
                }

                Node? find = await dbContext.Nodes.FirstOrDefaultAsync(x => x.node_name == topic);
                if (find != null)
                {
                    if (ArdConverter.TryConvertData(find.InfoTypeData, message, out object? value))
                    {
                        await CreateHistory(dbContext, find!, value!);
                    }
                    else
                    {
                        Log("NodeInfo : ⚠ node incorrect information of '" + topic + "' message is '" + message + "', type needed is : " + find.InfoTypeData.ToString());
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
            if (data is string str)
            {
                DataHistoryString dhs = CreateModel<DataHistoryString>(node);
                dhs.dh_string_value = str;
                await dbContext.DataHistoryStrings.AddAsync(dhs);
            }
            else if (data is DateTime dt)
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
                return;
            }
            await dbContext.WaitSaveChangesAsync();
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
                if (arrInfos.Length == 2)
                {
                    string nodeName = arrInfos[0];
                    string dataType = arrInfos[1];
                    if (!string.IsNullOrWhiteSpace(nodeName))
                    {
                        if (ArdConverter.IsAction(dataType, out InfoTypeData? type))
                        {
                            // CREATE ActionTrigger
                            ActionTrigger? find = await dbContext.ActionTriggers.FirstOrDefaultAsync(x => x.act_name == nodeName);
                            if(find == null)
                            {
                                ActionTrigger a = new ActionTrigger();
                                a.act_name = nodeName;
                                a.act_type_data = (int)type!;
                                nodeCreated = true;
                                await dbContext.ActionTriggers.AddAsync(a);
                            }
                        }else if(ArdConverter.IsEvent(dataType, out InfoTypeData? typeEvent))
                        {
                            if (!_event.ContainsKey(nodeName))
                            {
                                _event.Add(nodeName, typeEvent!.Value);
                                nodeCreated = true;
                            }
                        }
                        else if (ArdConverter.IsTypeData(dataType, out InfoTypeData? typeData) &&
                        typeData != InfoTypeData.Rbg)
                        {
                            // CREATE Node
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
                        else
                        {
                            Log("⚠ not correct type, 'nodeName' : " + nodeName + ", 'dataType' : " + dataType);
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

            if (!nodeCreated)
                Log("Already up to date");
            else
                await dbContext.WaitSaveChangesAsync();
            Log("--- End Init ---");
        }

        #endregion
    }
}
