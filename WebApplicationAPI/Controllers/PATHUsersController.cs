using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PATHServer.BDD.Models;
using PATHServer.Migrations;

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
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///GET/nodename
        ///  {
        ///     "name": "Guillemin",
        ///     "surname": "cedric",
        ///     "email": "mama@gmail.com",
        ///     "pass": "motdepassesécurisé"
        ///                    
        ///  }
        ///
        /// </remarks>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        [HttpPost("create")]
        public async Task<IActionResult> Create(string pass, string surname, string name, string email)
        {
            var u = new PATHUser();
            u.pu_surname = surname;
            u.pu_password= pass;
            u.pu_name = name;
            u.pu_email = email;
            u.pu_admin = false;
            _context.Add(u);
            await _context.SaveChangesAsync();
            return Ok(u);
        }

            
        /// <summary>
        /// connecte un utilisateur
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns>l'id de l'utilisateur connecté</returns>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        [HttpPost("connect")]
        public async Task<IActionResult> ConnectUser(string pass, string name)
        {
            PATHUser? nd = await _context.Users.FirstOrDefaultAsync(x => x.pu_name == name || x.pu_password == pass);

            List<data> _data = new List<data>();

            _data.Add(new data()
            {
                Id = nd.pu_id,
                SSN = 2,
                Message = "A Message"
            });

            string json = JsonSerializer.Serialize(_data);



            if (nd == null)
            {
                return Ok(Json("no User with this logs"));
            }
            else
            {
                return Ok(output);
            }
        }

        /// <summary>
        /// Modifie un utilisateur
        /// </summary>
        /// <param name="id">l'id nécéssaire pour retrouver le bon user</param>
        /// <param name="pass"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns>l'utilisateur modifié</returns>
        /// <response code="401">La clé de connexion a été refusé</response>
        /// <response code="400">Entrés invalide</response>
        [HttpPut("edit")]
        public async Task<IActionResult> EditUserInfo(int id, string? pass, string? surname, string? name, string? email)
        {
            PATHUser? u = await _context.Users.FirstOrDefaultAsync(x => x.pu_id == id);

            u.pu_surname = surname;
            u.pu_password = pass;
            u.pu_name = name;
            u.pu_email = email;
            u.pu_admin = false;

            _context.Update(u);
            await _context.SaveChangesAsync();
            if (u == null)
            {
                return Ok(Json("no User with this logs"));
            }
            else
            {
                return Ok(u);
            }
        }
    }
}
