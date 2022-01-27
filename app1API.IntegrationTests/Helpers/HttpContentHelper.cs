using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace app1API.IntegrationTests.Helpers
{
    public static class HttpContentHelper
    {
        public static HttpContent ToJsonHttpContent(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
