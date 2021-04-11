using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Authentication
{
    public class NococidAuthentication
    {
        private static readonly PasswordHasher<string> hasher = new PasswordHasher<string>();

        public static string GetHashedPassword(string username, string password)
        {
            return hasher.HashPassword(username, password);
        }

        public static bool VerifyHashedPassword(string username, string hashed_password, string password, out string rehashed_password)
        {
            rehashed_password = null;
            switch (hasher.VerifyHashedPassword(username, hashed_password, password))
            {
                case PasswordVerificationResult.SuccessRehashNeeded:
                    rehashed_password = GetHashedPassword(username, password);
                    return true;
                case PasswordVerificationResult.Success:
                    return true;
                default:
                    return false;
            }
        }
    }
}
