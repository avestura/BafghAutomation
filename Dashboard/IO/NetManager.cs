using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.IO
{
    public static class NetManager
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            var content = new FormUrlEncodedContent(data);

            var response = await client.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
