using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Common
{
    public class Crypto
    {
        public static string Encryption(string password)
        {
            return PasswordHash.ScryptHashString(password, PasswordHash.Strength.Medium);
        }

        public static bool CheckPassword(string password, string hash)
        {
            return PasswordHash.ScryptHashStringVerify(hash, password);
        }
    }
}
