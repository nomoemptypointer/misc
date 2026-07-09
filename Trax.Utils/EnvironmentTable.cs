using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trax.Utils
{
    public class EnvironmentTable
    {
        private const string envName = "BOT_TOKEN";
        public static string GetToken()
        {
            return Environment.GetEnvironmentVariable(envName) ?? throw new Exception($"Cannot login into the bot because there is no such field \"{envName}\" or it is empty.");
        }
    }
}
