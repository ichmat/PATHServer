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

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("node")]
    public class NodeController : Controller
    {
        private readonly MyDbContext _context;

        public NodeController(MyDbContext context)
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
        [HttpGet("action")]
        public async Task<IActionResult> ExecuteAction(string connexionId, string actionName, string actionData)
        {
            ActionTrigger? actionTrigger = _context.ActionTriggers.FirstOrDefault(x => x.act_name == actionName);
            if(actionTrigger != null)
            {
                if(Server.instance.IsValidData(actionTrigger!.ActTypeData, actionData, out string? val))
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
        /// Get All Node
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <returns></returns>
        [HttpGet("nodelist")]
        public async Task<IActionResult> GetNodeList(string connexionId)
        {
          List<Node> nd =  _context.Nodes.Select(row => row).ToList(); ;

            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(Json(nd));
            }
        }

        /// <summary>
        /// get a node by name
        /// </summary>
        /// <param name="connexionId">key connexio</param>
        /// <param name="nodename">node name</param>
        /// <remarks>
        /// Sample request:
        ///
        ///GET/nodename
        ///  {
        ///     "name": "Temperature",
        ///  }
        ///
        /// </remarks>
        /// <returns> </returns>
        [HttpGet("nodename")]
        public async Task<IActionResult> GetNodeByName(string connexionId, string nodename)
        {
            Node? nd = await _context.Nodes.FirstOrDefaultAsync(x => x.node_name == nodename);

            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(Json(nd));
            }
        }

        [HttpGet("nodeid")]
        public async Task<IActionResult> GetNodeById(string connexionId, int nodeid)
        {
            Node? nd = await _context.Nodes.FirstOrDefaultAsync(x => x.node_id == nodeid);

            if(nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(Json(nd));
            }
        }
    }
}
