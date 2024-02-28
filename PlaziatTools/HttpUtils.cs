﻿using System.Diagnostics;

namespace PlaziatTools
{

    /// <summary>
    /// Common http manipulations
    /// </summary>
    public static class HttpUtils
    {

        /// <summary>
        /// Open an hyperlink in default navigator
        /// </summary>
        /// <param name="link">Address</param>
        public static void OpenLink(string link)
        {
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }

        /// <summary>
        /// Retrieve response to an hyperlink
        /// Used to call public api
        /// </summary>
        /// <param name="link">Address</param>
        /// <returns>Response to the http call</returns>
        public static async Task<string> Get(string link)
        {
            var retour = string.Empty;
            try
            {
                using var httpClient = new HttpClient();
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, link);
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
