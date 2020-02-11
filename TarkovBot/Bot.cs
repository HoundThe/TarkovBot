using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TarkovBot
{
    public sealed class Bot
    {
        private static TarkovHttpClient tarkovHttpClient;
        private Dictionary<string, string> itemsDictionary;
        private PlayerData playerData;

        public Bot(string session)
        {
            var cookies = new CookieContainer();

            cookies.Add(new Uri(TarkovHttpClient.PROD_ENDPOINT), new Cookie("PHPSESSID", session));
            cookies.Add(new Uri(TarkovHttpClient.RAGFAIR_ENDPOINT), new Cookie("PHPSESSID", session));

            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                UseCookies = true,
                ClientCertificateOptions = ClientCertificateOption.Automatic
            };

            tarkovHttpClient = new TarkovHttpClient(session, handler);

            itemsDictionary = new Dictionary<string, string>();
            itemsDictionary.Add("rouble", "5449016a4bdc2d6f028b456f");
        }
        
        public async Task StartAsync()
        {
            playerData = await tarkovHttpClient.Auth();
            /**
            
            Your logic

             */
            await tarkovHttpClient.KeepAlive();
        }
    }
}
