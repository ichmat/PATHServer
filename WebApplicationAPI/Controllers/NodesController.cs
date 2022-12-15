using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        /// <param name="pageSize">Page size</param>
        /// <param name="pageNumber">Index page</param>
        /// <returns>list of node</returns>
        /// <response code="401">Error with connexonId</response>
        /// <response code="400">Invalid entries</response>
        [HttpGet("list")]
        public async Task<IActionResult> GetNodeList(string connexionId, int? pageSize, int? pageNumber)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/list - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);
            List<Node> nodeList = new();
            List<NodeParsed> parsedNodeList = new();
            try
            {
                 nodeList = PathTools.PaginationFilter(_context.Nodes, pageNumber, pageSize);
               
            }
            catch
            {
                return await LogsResult.LogAndResult("node/list - index outside of list", TypeLOG.ERROR, connexionId, _context, Ok, PathTools.GetJsonResponse("index outside of list"));
            }

            foreach (Node node in nodeList)
            {
                parsedNodeList.Add((NodeParsed)PathTools.ToParsed(node));
            }

            if (parsedNodeList == null)
            {
                return await LogsResult.LogAndResult("node/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("no node"));
            }
            else
            {
                return await LogsResult.LogAndResult("node/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(parsedNodeList, "Ok"));
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
                return await LogsResult.LogAndResult("node/name - no node found with this name", TypeLOG.WARNING, connexionId, _context, NotFound);
            }
            else
            {

                NodeParsed np = PathTools.ToParsed((nd));
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
        /// <response code="401">Error with connexonId</response>
        /// <response code="400">Invalid entries</response>
        [HttpGet("id")]
        public async Task<IActionResult> GetNodeById(string connexionId, int nodeid)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/id - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            Node? nd = await _context.Nodes.FirstOrDefaultAsync(x => x.node_id == nodeid);
            if (nd == null)
            {
                return await LogsResult.LogAndResult("node/name - no node found with this id", TypeLOG.WARNING, connexionId, _context, NotFound);
            }
            else
            {
                NodeParsed parsedNode = PathTools.ToParsed(nd);
                return await LogsResult.LogAndResult("node/id - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(parsedNode, "Ok"));
            }
        }

        /// <summary>
        /// Get the last data of this node
        /// </summary>
        /// <param name="connexionId">connexion key</param>
        /// <param name="nodeid">id node</param>
        /// <returns>last data found</returns>
        /// <response code="401">Error with connexonId</response>
        /// <response code="400">Invalid entries</response>
        [HttpGet("lastdata")]
        public async Task<IActionResult> GetLastDataByNodeId(string connexionId, int nodeid)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("node/lastdata - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            Node? nd = _context.Nodes.FirstOrDefault(x => x.node_id == nodeid);
            if (nd == null)
            {
                return await LogsResult.LogAndResult("node/lastdata - no node found with this id", TypeLOG.FAIL, connexionId, _context, NotFound);
            }
            else
            {
                DataHistory? dataHistory = GetLastDataOfNode(_context, nd);
                if(dataHistory == null)
                {
                    return await LogsResult.LogAndResult("node/lastdata - no data with this node", TypeLOG.WARNING, connexionId, _context, Ok, PathTools.GetJsonResponse("no data with this nod"));
                }
                else
                {
                    return await LogsResult.LogAndResult("node/lastdata - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(PathTools.ToParsed(dataHistory), "Ok"));
                }
            }
        }

        internal static DataHistory? GetLastDataOfNode(MyDbContext _context, Node node)
        {
            return _context.DataHistories.Where(x => x.node_id == node.node_id).OrderByDescending(x => x.dh_date).FirstOrDefault();
        }
    }
}
