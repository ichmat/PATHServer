using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PATHServer.BDD.Models;
using PATHServer;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("init")]
    public class InitController : Controller
    {
        private readonly MyDbContext _context;

        public InitController(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Set Wifi credential
        /// </summary>
        /// <param name="ssid">WIFI Name</param>
        /// <param name="password">WIFI password</param>
        /// <returns></returns>
        [HttpPost("creds")]
        public async Task<IActionResult> WIFICredentials(string ssid, string password)
        {
            if (!Server.isDefaultWifi)
            {
                return await LogsResult.LogAndResult("init/creds - can't set wifi credential", TypeLOG.WARNING, string.Empty, _context, BadRequest, PathTools.GetJsonResponse("can't set wifi credential"));
            }
            Server.instance.SetCredentialWifi(ssid, password);
            RestartProgram();
            return await LogsResult.LogAndResult("init/creds - OK", TypeLOG.SUCCESS, string.Empty, _context, BadRequest, PathTools.GetJsonResponse("OK"));
        }

        private async void RestartProgram()
        {
            await Task.Delay(3000);
            System.Diagnostics.Process.Start("dotnet","WebApplicationAPI.dll");
            Program.lifetime.StopApplication();
        }
    }
}
