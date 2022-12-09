using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        /// Créer une node
        /// </summary>
        /// <param name="connexionId">La clé de connexion</param>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns>l'id d'utilisateur</returns>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        [HttpPost("create")]
        public async Task<IActionResult> Create(int node_id, string node_name)
        {
            var u = new Node();
            u.node_name = node_name;
            u.node_id = node_id;
            _context.Add(u);
            await _context.SaveChangesAsync();
            return Ok(u);
        }

        /// <summary>
        /// get list of node
        /// </summary>
        /// <returns></returns>
        [HttpGet("nodelist")]
        public async Task<IActionResult> GetNodeList()
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
        public async Task<IActionResult> GetNodeByName(string nodename)
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

        /// <summary>
        /// Créer une node
        /// </summary>
        /// <param name="nodeid">L'id Correspondant au capteur</param>
        /// <remarks>
        /// <returns>l'id d'utilisateur</returns>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        /// Sample request:
        ///
        /// GET/nodeid
        /// {
        ///    "id": "1"
        /// }
        ///
        /// </remarks>
        [HttpGet("nodeid")]
        public async Task<IActionResult> GetNodeById(int nodeid)
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
