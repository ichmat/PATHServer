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
        /// <returns>list of node</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetNodeList(string connexionId)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/list - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            List<NodeParsed> nd = _context.Nodes.Select(row => NodeParsed.CreateFromModel(row)).ToList();
            
            if (nd == null)
            {
                return await LogsResult.LogAndResult("node/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("no node"));
            }
            else
            {
                return await LogsResult.LogAndResult("node/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(nd, "Ok"));
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
        ///GET/node/name
        ///  {
        ///     "name": "Temperature",
        ///  }
        ///
        /// </remarks>
        /// <returns> </returns>
        [HttpGet("name")]
        public async Task<IActionResult> GetNodeByName(string connexionId, string nodename)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/name - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            Node? nd = _context.Nodes.FirstOrDefault(row => row.node_name == nodename);
            if (nd == null)
            {
                return await LogsResult.LogAndResult("node/name - no node found with this name", TypeLOG.FAIL, connexionId, _context, NotFound);
            }
            else
            {
                NodeParsed np = NodeParsed.CreateFromModel(nd);
                return await LogsResult.LogAndResult("node/name - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(np, "Ok"));
            }
        }

        /// <summary>
        /// get a node by id
        /// </summary>
        /// <param name="connexionId">key connexio</param>
        /// <param name="nodeid">node id</param>
        /// <remarks>
        /// Sample request:
        ///
        ///GET/node/id
        ///  {
        ///     "name": "Temperature",
        ///  }
        ///
        /// </remarks>
        /// <returns> the node </returns>
        [HttpGet("id")]
        public async Task<IActionResult> GetNodeById(string connexionId, int nodeid)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/id - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            Node? nd = await _context.Nodes.FirstOrDefaultAsync(x => x.node_id == nodeid);
            if (nd == null)
            {
                return await LogsResult.LogAndResult("node/name - no node found with this id", TypeLOG.FAIL, connexionId, _context, NotFound);
            }
            else
            {
                NodeParsed parsedNode = NodeParsed.CreateFromModel(nd);
                return await LogsResult.LogAndResult("node/id - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(parsedNode, "Ok"));
            }
        }
    }
}
