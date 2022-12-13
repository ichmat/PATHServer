using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PATHServer;
using PATHServer.BDD.Models;
using PATHServer.Migrations;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("user")]
    public class PATHUsersController : Controller
    {
        private readonly MyDbContext _context;

        public PATHUsersController(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create an user
        /// </summary>
        /// <param name="connexionId">connexion key</param>
        /// <param name="surname">surname of the new user</param>
        /// <param name="name">name of the new user</param>
        /// <param name="email">email of the new user</param>
        /// <param name="password">password of the user</param>
        /// <returns>the user created</returns>
        /// 
        /// <remarks>
        /// Sample request:
        ///
        ///GET/nodename
        ///  {
        ///     "name": "Guillemin",
        ///     "surname": "Cédric",
        ///     "email": "mama@gmail.com",
        ///     "pass": "motdepassesécurisé"
        ///                    
        ///  }
        ///
        /// </remarks>
        /// <response code="401">Error with connexonId</response>
        /// <response code="400">Invalid entries</response>
        [HttpPost("create")]
        public async Task<IActionResult> Create(string connexionId, string surname, string name, string email, string password)
        {
            bool no_user = _context.Users.Count() == 0;

            if (no_user! && await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("user/create - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            var u = new PATHUser();
            u.pu_surname = surname;
            u.pu_password = password;
            u.pu_name = name;
            u.pu_email = email;

            if (no_user)
            {
                u.pu_admin = true;
            }
            else
            {
                u.pu_admin = false;
            }
            
            _context.Add(u);
            await _context.SaveChangesAsync();
            return await LogsResult.LogAndResult("user/create - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathUserParsed.CreateFromModel(u));
        }

        /// <summary>
        /// attempts to connect
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="password">password of the user</param>
        /// <returns>The key of connexion</returns>
        /// <response code="400">Invalid entries</response>
        [HttpPost("connect")]
        public async Task<IActionResult> ConnectUser(string email, string password)
        {
            PATHUser? nd = await _context.Users.FirstOrDefaultAsync(x => x.pu_email == email && x.pu_password == password);

            if (nd == null)
            {
                return await LogsResult.LogAndResult("user/connect - bad credentials", TypeLOG.FAIL, "", _context, BadRequest, PathTools.GetJsonResponse("bad credentials"));
            }
            else
            {
                KeyConnexion c = new KeyConnexion();
                c.key_id = Guid.NewGuid().ToString();
                c.pu_id = nd.pu_id;
                c.key_quota = 0;
                c.key_quotaRefresh = DateTime.Now;
                c.key_lastUpdated = DateTime.Now;
                c.key_created = DateTime.Now;
                _context.Add(c);
                await _context.SaveChangesAsync(true);
                return await LogsResult.LogAndResult("user/connect - OK", TypeLOG.SUCCESS, c.key_id, _context, Ok, PathTools.GetJsonResponse(c.key_id, ""));
            }
        }

        /// <summary>
        /// Modifie un utilisateur
        /// </summary>
        /// <param name="connexionId">connexion key</param>
        /// <param name="id">the id to find the user</param>
        /// <param name="pass">the new password</param>
        /// <param name="surname">the new surname</param>
        /// <param name="email">the new email</param>
        /// <param name="name">the new name</param>
        /// <returns>the user edited</returns>
        /// <response code="401">key connexion refused or not the good one</response>
        /// <response code="400">invalid entries</response>
        [HttpPut("edit")]
        public async Task<IActionResult> EditUserInfo(string connexionId, int id, string? pass, string? surname, string? name, string? email)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("user/edit - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            PATHUser? u = await _context.Users.FirstOrDefaultAsync(x => x.pu_id == id);

            if (u == null)
            {
                return await LogsResult.LogAndResult("user/edit - no user with this id", TypeLOG.FAIL, connexionId, _context, BadRequest);
            }
            else
            {
                if (pass != null)
                    u.pu_password = pass;
                if(surname != null)
                    u.pu_surname = surname;
                if(name != null)
                    u.pu_name = name;
                if(email != null)
                    u.pu_email = email;

                _context.Update(u);
                await _context.SaveChangesAsync();
                return await LogsResult.LogAndResult("user/edit - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(PathUserParsed.CreateFromModel(u), "success editing"));
            }
           
        }
    }
}
