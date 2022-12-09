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
        [HttpGet("execute")]
        public async Task<IActionResult> ExecuteAction(string connexionId, string actionName, string actionData)
        {
            ActionTrigger? actionTrigger = _context.ActionTriggers.FirstOrDefault(x => x.act_name == actionName);
            if (actionTrigger != null)
            {
                if (Server.instance.IsValidData(actionTrigger!.ActTypeData, actionData, out string? val))
                {
                    await Server.instance.SendBroadcast(actionTrigger!.act_name, val);
                    // CHECK SENDING
                    return Ok();
                }
                else
                {
                    return BadRequest("bad type data for 'actionData'");
                }
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get all actions that user can do
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <returns>JSON of all <see cref="ActionTrigger"/></returns>
        [HttpGet("actionlist")]
        public async Task<IActionResult> GetActionList(string connexionId)
        {
            List<ActionTrigger> nd = _context.ActionTriggers.Select(row => row).ToList();

            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                List<ActionTriggerParsed> triggerParseds = nd.ConvertAll(x => ActionTriggerParsed.CreateFromModel(x));
                return Ok(Json(triggerParseds));
            }
        }
    }
}
