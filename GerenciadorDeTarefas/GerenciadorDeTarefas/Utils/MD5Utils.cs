using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Utils
{
    public class MD5Utils
    {
        public static string GerarHashMD5(string input)
        {
            MD5 md5Hash = MD5.Create();
            var bytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            foreach(var b in bytes)
            {
                sBuilder.Append(b.ToString("X2"));
            }

            return sBuilder.ToString();
        }
    }
}
