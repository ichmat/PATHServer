using Microsoft.EntityFrameworkCore;
using PATHServer.BDD.Models;
using PATHServer.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ArduinoAction
{
    public static class ActionIdentifier
    {
        private static readonly Dictionary<string,int> caches_actionHistory = new Dictionary<string,int>();

        private static readonly Dictionary<string, string> actionsNames = new Dictionary<string,string>();

        public static async Task<int> GetActionHistoryInfo(MyDbContext _content, string actionName, string actionData)
        {
            string action = actionName + actionData;
            if (caches_actionHistory.TryGetValue(action, out int id_actionInfo))
                return id_actionInfo;


            string actionTranslated ;
            if (actionsNames.ContainsKey(action))
            {
                actionTranslated = actionsNames[action];
            }
            else
            {
                if (actionsNames.ContainsKey(actionName))
                {
                    actionTranslated = actionsNames[actionName];
                }
                else
                {
                    actionTranslated = action;
                }
            }

            ActionHistoryInfo? info = await _content.ActionHistoryInfos.FirstOrDefaultAsync(x => x.ahi_name == actionTranslated);
            if (info != null)
            {
                caches_actionHistory.Add(action, info.ahi_id);
                return info.ahi_id;
            }
            else
            {
                ActionHistoryInfo newinfo = new ActionHistoryInfo();
                newinfo.ahi_name = actionTranslated;
                await _content.AddAsync(newinfo);
                await _content.WaitSaveChangesAsync();
                newinfo = await _content.ActionHistoryInfos.FirstAsync(x => x.ahi_name == actionTranslated);
                caches_actionHistory.Add(action, newinfo.ahi_id);
                return newinfo.ahi_id;
            }
        }

        public static void Init()
        {
            actionsNames.Clear();
            actionsNames.Add("door1", "Porte ouverte");
            actionsNames.Add("door0", "Porte fermée");
            actionsNames.Add("light", "Lumière");
            actionsNames.Add("window0", "Fenêtre fermée");
            actionsNames.Add("window1", "Fenêtre ouverte");
            actionsNames.Add("heating1", "Chauffage allumé");
            actionsNames.Add("heating0", "Chauffage éteint");
            actionsNames.Add("fan1", "Ventilateur/Climatisation allumé(e)");
            actionsNames.Add("fan0", "Ventilateur/Climatisation éteint");
            actionsNames.Add("alarm1", "Alarme allumée");
            actionsNames.Add("alarm0", "Alarme éteint");
        }
    }
}
