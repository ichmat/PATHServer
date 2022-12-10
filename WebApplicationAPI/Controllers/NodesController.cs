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
        /// Get All Node
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <returns></returns>
        [HttpGet("nodelist")]
        public async Task<IActionResult> GetNodeList(string connexionId)
        {
          List<NodeParsed> nd = _context.Nodes.Select(row => NodeParsed.CreateFromModel(row)).ToList();
            
            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(PathTools.GetJsonResponse(nd, "Ok"));
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
            Node nd = _context.Nodes.First(row => row.node_name == nodename);
            NodeParsed tat = NodeParsed.CreateFromModel(nd);
            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(PathTools.GetJsonResponse(tat, "Ok"));
            }
        }

        [HttpGet("nodeid")]
        public async Task<IActionResult> GetNodeById(string connexionId, int nodeid)
        {
            Node? nd = await _context.Nodes.FirstOrDefaultAsync(x => x.node_id == nodeid);
            var parsedNode = NodeParsed.CreateFromModel(nd);
            if (nd == null)
            {
                return Ok(Json("no node"));
            }
            else
            {
                return Ok(PathTools.GetJsonResponse(parsedNode, "Ok"));
            }
        }
    }
}
