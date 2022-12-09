using System;
using System.Collections.Generic;
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
    [Route("user")]
    public class NoderController : Controller
    {
        private readonly MyDbContext _context;

        public NoderController(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Créer un utilisateur
        /// </summary>
        /// <param name="connexionId">La clé de connexion</param>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns>l'id d'utilisateur</returns>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        [HttpPost("create")]
        public async Task<IActionResult> Create(string connexionId, string surname, string name, string email)
        {
            var u = new PATHUser();
            u.pu_surname = surname;
            u.pu_name = name;
            u.pu_email = email;
            u.pu_admin = false;
            _context.Add(u);
            await _context.SaveChangesAsync();
            return Ok(u);
        }

        /// <summary>
        /// Fait un simple ping
        /// </summary>
        /// <returns></returns>
        [HttpGet("connect")]
        public async Task<IActionResult> ConnectUser(string surname, string name, string email)
        {
            Node? nd = await _context.Nodes.FirstOrDefaultAsync();
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
