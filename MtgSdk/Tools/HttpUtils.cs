using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace MaGeek.Framework.Data
{

    public static class HttpUtils
    {

        public static async Task<string> Get(string v)
        {
            var retour = string.Empty;
            try
            {
                using var httpClient = new HttpClient();
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, v);
                    var response = await httpClient.SendAsync(request);

                    using var reader = new StreamReader(response.Content.ReadAsStream());
                    retour = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Http Get failed : " + e.Message);
            }
            return retour;
        }

    }

}
