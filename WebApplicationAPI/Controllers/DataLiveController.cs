using Microsoft.AspNetCore.Mvc;
using PATHServer.BDD.Models;
using PATHServer.ModelParser;
using PATHServer;
using PATHServer.ArduinoAction;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("live")]
    public class DataLiveController : Controller
    {
        private readonly MyDbContext _context;

        public DataLiveController(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all actions that user can do
        /// </summary>
        /// <param name="connexionId">Key connexion</param>
        /// <param name="indexStart">Index page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>JSON of <see cref="DataLive"/></returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetLiveList(string connexionId, int? indexStart, int? pageSize)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("live/list - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            List<DataLive> dl = _context.DataLives.ToList();

            try
            {
                dl = PathTools.PaginationFilter(_context.DataLives, indexStart, pageSize);
            }
            catch (ArgumentException ex)
            {
                return await LogsResult.LogAndResult("live/list - " + ex.Message, TypeLOG.ERROR, connexionId, _context, BadRequest, PathTools.GetJsonResponse(ex.Message));
            }

            if (dl == null || dl.Count == 0)
            {
                return await LogsResult.LogAndResult("live/list - no data live", TypeLOG.WARNING, connexionId, _context, Ok, PathTools.GetJsonResponse("no data live"));
            }
            else
            {
                var parsed = dl.ConvertAll(x => (DataLiveParsed)PathTools.ToParsed(x));
                return await LogsResult.LogAndResult("live/list - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse(parsed, "Success : List of LiveData"));
            }
        }

        /// <summary>
        /// Publish a data live
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <param name="liveName">the name</param>
        /// <param name="liveData">the data to publish</param>
        /// <returns></returns>
        [HttpPost("publish")]
        public async Task<IActionResult> Publish(string connexionId, string liveName, string liveData)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("live/publish - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            string reasonFail = await DataLiveManager.TryPublish(_context, liveName, liveData);

            if (reasonFail == string.Empty)
            {
                return await LogsResult.LogAndResult("live/publish - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("Ok"));
            }
            else
            {
                return await LogsResult.LogAndResult("live/publish - " + reasonFail, TypeLOG.FAIL, connexionId, _context, BadRequest, PathTools.GetJsonResponse(reasonFail));
            }
        }

        /// <summary>
        /// Unset a data live
        /// </summary>
        /// <param name="connexionId">key connexion</param>
        /// <param name="liveName">the name</param>
        /// <returns></returns>
        [HttpDelete("unset")]
        public async Task<IActionResult> Unset(string connexionId, string liveName)
        {
            if (await PathTools.CheckKey(_context, connexionId) == false)
                return await LogsResult.LogAndResult("live/unset - invalid key", TypeLOG.FAIL, connexionId, _context, Unauthorized);

            string reasonFail = await DataLiveManager.TryUnset(_context, liveName);

            if (reasonFail == string.Empty)
            {
                return await LogsResult.LogAndResult("live/unset - OK", TypeLOG.SUCCESS, connexionId, _context, Ok, PathTools.GetJsonResponse("Ok"));
            }
            else
            {
                return await LogsResult.LogAndResult("live/unset - " + reasonFail, TypeLOG.FAIL, connexionId, _context, BadRequest, PathTools.GetJsonResponse(reasonFail));
            }
        }
    }
}
