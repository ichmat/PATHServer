using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationAPI;

namespace PATHServer
{
    public class PathTools
    {
        public static string GetJsonResponse(object obj, string message)
        {

            object rt = new
            {
                Object = obj,
                Message = message
            };

            return JsonConvert.SerializeObject(rt);
        }

        public static string GetJsonResponse(string message)
        {

            object rt = new
            {
                Message = message
            };

            return JsonConvert.SerializeObject(rt);
        }

        public string GenerateEncryptedPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(PATHUser user, string password) => BCrypt.Net.BCrypt.Verify(password, user.pu_password);

        private static readonly TimeSpan EXPIRATION_TIME = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan QUOTA_REFRESH = TimeSpan.FromMinutes(1);
        private static readonly int QUOTA_LIMIT = 50;

        public async static Task<PATHUser?> GetUserByConnexionKey(MyDbContext _context, string connexionKey)
        {
            KeyConnexion? currentkey = await _context.Keys.FirstOrDefaultAsync(x => x.key_id == connexionKey);
            if (currentkey == null) return null;
            return await _context.Users.FirstOrDefaultAsync(x => x.pu_id == currentkey.pu_id);

        }

        public async static Task<bool> CheckKey(MyDbContext _context, string connexionId)
        {
            if (string.IsNullOrWhiteSpace(connexionId)) return false;

            // Vérification de la clef 
            KeyConnexion? currentkey = await _context.Keys.FirstOrDefaultAsync(x => x.key_id == connexionId);
            if (currentkey == null) // La clé n'existe pas 
            {
                return false;
            }

            TimeSpan quotaRefresh = DateTime.Now.ToUniversalTime() - currentkey.key_quotaRefresh.ToUniversalTime();

            TimeSpan lifeTimeKey = DateTime.Now.ToUniversalTime() - currentkey.key_lastUpdated.ToUniversalTime();

            //Verifie si la clé a expiré
            if (lifeTimeKey > EXPIRATION_TIME)
            {
                // La clé a expiré
                return false;
            }
            else if (quotaRefresh < QUOTA_REFRESH && currentkey.key_quota > QUOTA_LIMIT)
            {
                // quota de modification dépassé
                return false;
            }
            else
            {
                if (quotaRefresh > QUOTA_REFRESH)
                {
                    // reset du quota
                    currentkey.key_quotaRefresh = DateTime.Now.ToUniversalTime();
                    currentkey.key_quota = 1;
                }
                else
                {
                    // incrémentation du quota
                    currentkey.key_quota++;
                }
                currentkey.key_lastUpdated = DateTime.Now.ToUniversalTime(); // rafraîchit la validité de la clé
                try
                {
                    _context.Update(currentkey);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }

                return true;
            }
        }

        public async static Task<string> GetReasonKeyFail(MyDbContext _context, string connexionId)
        {
            if (string.IsNullOrWhiteSpace(connexionId)) return "empty key";
            // LE QUOTA N'EST PAS DEPASSE toutes les 1 min? 

            // Vérification de la clef 
            KeyConnexion? currentkey = await _context.Keys.FirstOrDefaultAsync(x => x.key_id == connexionId);
            if (currentkey == null) // La clé n'existe pas 
            {
                return "key not exist";
            }

            TimeSpan quotaRefresh = DateTime.Now.ToUniversalTime() - currentkey.key_quotaRefresh.ToUniversalTime();

            TimeSpan lifeTimeKey = DateTime.Now.ToUniversalTime() - currentkey.key_lastUpdated.ToUniversalTime();

            //Verifie si la clé a expiré
            if (lifeTimeKey > EXPIRATION_TIME)
            {
                // La clé a expiré
                return "key expired";
            }
            else if (quotaRefresh < QUOTA_REFRESH && currentkey.key_quota > QUOTA_LIMIT)
            {
                // quota de modification dépassé
                return "quota exceded";
            }
            else
            {
                return "no error detected, quota : " + currentkey.key_quota + " ( max : " + QUOTA_LIMIT + " , refresh time : " + QUOTA_REFRESH.ToString("g") + " ), " +
                    "expiration in " + lifeTimeKey.ToString("g") + " ( expiration duration : " + EXPIRATION_TIME.ToString("g") + " )";
            }
        }
    }
}
