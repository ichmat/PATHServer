using Microsoft.AspNetCore.Mvc;
using PATHServer;
using PATHServer.ArduinoAction;
using PATHServer.BDD.Models;

namespace WebApplicationAPI.Controllers
{
    public static class LogsResult
    {
        internal async static Task<bool> SaveActionAsHistory(MyDbContext _context, ActionTrigger action, string connexionKey, string actionName, string actionData)
        {
            PATHUser? user = await PathTools.GetUserByConnexionKey(_context, connexionKey);
            if(user != null)
            {
                ActionHistory history = new ActionHistory();
                history.ah_date = DateTime.Now;
                history.pu_id = user.pu_id;
                history.ahi_id = await ActionIdentifier.GetActionHistoryInfo(_context, actionName, actionData);
                history.act_id = action.act_id;
                await _context.AddAsync(history);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal async static Task<IActionResult> InternalError(Exception ex, string ou, string connexionKey, MyDbContext _context, Func<int, object?, IActionResult> result)
        {
            try
            {
                PATHUser? user = await  PathTools.GetUserByConnexionKey(_context, connexionKey);
                Log l = Log.GenerateLog(ou + " error message : " + ex.Message + " Stacktrace : " + ex.StackTrace, TypeLOG.FATAL, user != null ? user.pu_id : null);
                _context.Add(l);
                await _context.WaitSaveChangesAsync(true);
                return result!.Invoke(500, ex);
            }
            catch
            {
                return result!.Invoke(500, ex);
            }
        }

        internal async static Task<IActionResult> LogAndResult(string quoi, TypeLOG type, string? connexionKey, MyDbContext _context, Func<object, IActionResult> result)
        {
            PATHUser? user = await PathTools.GetUserByConnexionKey(_context, connexionKey);
            string reason = quoi;
            if (quoi.Contains("invalid key"))
            {
                reason = quoi.Split('-')[0] + "- " + await PathTools.GetReasonKeyFail(_context, connexionKey);
            }
            Log l = Log.GenerateLog(reason, type, user != null ? user.pu_id :null);
            _context.Add(l);
            await _context.WaitSaveChangesAsync(true);
            return result!.Invoke(reason);
        }

        internal async static Task<IActionResult> LogAndResult(string quoi, TypeLOG type, string connexionKey, MyDbContext _context, Func<object, IActionResult> result, object objToReturn)
        {
            PATHUser? user = await PathTools.GetUserByConnexionKey(_context, connexionKey);
            Log l = Log.GenerateLog(quoi, type, user != null ? user.pu_id : null);
            _context.Add(l);
            await _context.WaitSaveChangesAsync(true);
            return result!.Invoke(objToReturn);
        }
    }
}
