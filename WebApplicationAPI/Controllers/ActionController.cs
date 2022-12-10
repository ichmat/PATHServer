using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PATHServer;
using PATHServer.BDD.Models;
using PATHServer.ModelParser;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("action")]
    public class ActionController : Controller
    {
        private readonly MyDbContext _context;

        public ActionController(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Execute an action
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <param name="actionName">name of the action</param>
        /// <param name="actionData">data for the action</param>
        /// <returns></returns>
        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteAction(string connexionId, string actionName, string actionData)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("action/execute - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            ActionTrigger? actionTrigger = _context.ActionTriggers.FirstOrDefault(x => x.act_name == actionName);
            if (actionTrigger != null)
            {
                if (Server.instance.IsValidData(actionTrigger!.ActTypeData, actionData, out string? val))
                {
                    await Server.instance.SendBroadcast(actionTrigger!.act_name, val!);
                    // AWAIT CHECK SENDING
                    return await LogsResult.LogAndResult("action/execute - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(actionTrigger, "Action done"));
                }
                else
                {
                    return await LogsResult.LogAndResult("action/execute - bad type data for 'actionData'", TypeLOG.FAIL, connexionId, _context, BadRequest);
                }
            }
            else
            {
                return await LogsResult.LogAndResult("action/execute - action not found", TypeLOG.FAIL, connexionId, _context, NotFound);
            }
        }

        /// <summary>
        /// Get all actions that user can do
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <returns>JSON of all <see cref="ActionTrigger"/></returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetActionList(string connexionId)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("action/list - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            List<ActionTrigger> nd = _context.ActionTriggers.Select(row => row).ToList();

            if (nd == null)
            {
                return await LogsResult.LogAndResult("action/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("no node"));
            }
            else
            {
                List<ActionTriggerParsed> triggerParseds = nd.ConvertAll(x => ActionTriggerParsed.CreateFromModel(x));
                return await LogsResult.LogAndResult("action/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(triggerParseds, "Success : List of Action "));
            }
        }

        /// <summary>
        /// get historic of the actions made by a user
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <param name="id">if of the user</param>
        /// <returns></returns>
        [HttpGet("history")]
        public async Task<IActionResult> GetActionHistory(string connexionId, int id)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("action/history - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            List<ActionHistory> nd = _context.ActionHistories.Select(row => row).Where(row => row.pu_id == id).ToList();

            if (nd == null)
            {
                return await LogsResult.LogAndResult("action/history - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("Empty Data"));
            }
            else
            {
                return await LogsResult.LogAndResult("action/history - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(nd, "Sucess List of History"));
            }
        }
    }
}
