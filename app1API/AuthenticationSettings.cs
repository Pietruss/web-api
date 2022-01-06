using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app1API
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public int JwtExpireDays { get; set; }
        public string JwtIssuer { get; set; }
    }
}
