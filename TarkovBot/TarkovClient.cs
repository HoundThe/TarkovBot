using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace TarkovBot
{
    public class TarkovHttpClient : HttpClient
    {
        public static string LAUNCHER_ENDPOINT = "https://launcher.escapefromtarkov.com";
        public static string PROD_ENDPOINT = "https://prod.escapefromtarkov.com";
        public static string TRADING_ENDPOINT = "https://trading.escapefromtarkov.com";
        public static string RAGFAIR_ENDPOINT = "https://ragfair.escapefromtarkov.com";

        private const string BSG_LAUNCHER_VERSION = "0.9.2.970";
        private const string EFT_CLIENT_VERSION = "0.12.3.5776";
        private const string UNITY_VERSION = "2018.4.13f1";

        public TarkovHttpClient(string session, HttpMessageHandler handler) : base(handler)
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.Add("User-Agent", $"UnityPlayer/{UNITY_VERSION} (UnityWebRequest/1.0, libcurl/7.52.0-DEV)");
            DefaultRequestHeaders.Add("App-Version", $"EFT Client {EFT_CLIENT_VERSION}");
            DefaultRequestHeaders.Add("X-Unity-Version", UNITY_VERSION);
        }

        public async Task<T> PostJson<T>(string uri, string body)
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            content.Headers.ContentType.CharSet = "";

            var response = await PostAsync(uri, content).ConfigureAwait(false);

            byte[] data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            if((int)response.StatusCode != 200)
                Console.WriteLine("Status code: " + (int)response.StatusCode);

            var decodedData = await DeflateResponse(data);

            if (typeof(T) == typeof(string))
                return (T)(object)decodedData;
            else
                return JsonConvert.DeserializeObject<T>(decodedData);
        }
        public async Task<Datum> Auth()
        {
            var profile = await PostJson<Profile>(PROD_ENDPOINT + "/client/game/profile/list", "");
            var playerData = profile.data?.FirstOrDefault(x => x.Info.Side != "Savage");

            if (playerData == null)
            {
                Console.WriteLine("Player data not found");
                return null;
            }

            Console.WriteLine("Level: " + playerData.Info.Level);

            await PostJson<string>(PROD_ENDPOINT + "/client/game/profile/select", $" {{ \"uid\":  \"{playerData._id}\" }}");

            return playerData;
        }

        private async Task<string> DeflateResponse(byte[] data)
        {
            byte[] decompressedData = new byte[data.Length * 100];
            int decompressedLength = 0;

            using (MemoryStream memory = new MemoryStream(data))
            using (InflaterInputStream inflater = new InflaterInputStream(memory))
                decompressedLength = await inflater.ReadAsync(decompressedData, 0, decompressedData.Length);

            return Encoding.Default.GetString(decompressedData);
        }

        public async Task KeepAlive()
        {
            while (true)
            {
                Console.WriteLine("Heartbeat");
                await PostJson<string>(PROD_ENDPOINT + "/client/game/keepalive", "");
                await Task.Delay(25000);
            }
        }
        public async Task<Price> GetItemPrice(string id)
        {
            var body = new GetPriceRequest { templateId = id };
            return await PostJson<Price>($"{RAGFAIR_ENDPOINT}/client/ragfair/itemMarketPrice", JsonConvert.SerializeObject(body));
        }

        public async Task<BuyStatus> BuyItem(string offerId, ulong quantity, List<BarterItem> items)
        {
            var body = new MoveFleaItemRequest
            {
                data = new List<BuyItemRequest>
                {
                    new BuyItemRequest
                    {
                         Action = "RagFairBuyOffer",
                         offers = new List<BuyOffer>{
                             new BuyOffer
                              {
                            id = offerId,
                            count = quantity,
                            items = items
                              }
                         }
                    }
                },
                tm = 2
            };
            var response = await PostJson<BuyItemResponse>($"{PROD_ENDPOINT}/client/game/profile/items/moving", JsonConvert.SerializeObject(body));
            return HandleBuyErrors(response);

        }

        private BuyStatus HandleBuyErrors(BuyItemResponse response)
        {
            if (response.errmsg != null)
            {
                Logger.Log(response.errmsg, LoggingLevel.Verbose);
                return BuyStatus.OtherError;
            }
            else
            {
                if (response.data.badRequest.Count != 0)
                {
                    var errorMessage = response.data.badRequest.First().errmsg;
                    Logger.Log(errorMessage, LoggingLevel.Verbose);
                    if (errorMessage.Contains("not found"))
                        return BuyStatus.OfferNotFound;
                    if (errorMessage.Contains("place"))
                        return BuyStatus.InventoryFull;
                    if (errorMessage.Contains("locked"))
                        return BuyStatus.ProfileLocked;
                    if (errorMessage.Contains("has count"))
                        return BuyStatus.NotEnoughMoney;

                    return BuyStatus.OtherError;
                }
                else
                {
                    Logger.Log("Successful buy", LoggingLevel.Verbose);
                    return BuyStatus.Success;
                }
            }
        }

        public async Task SellItem(List<string> items, bool sellAll, Requirement requirement)
        {
            var body = new SellFleaItemRequest
            {
                data = new List<SellItemRequest>
                {
                    new SellItemRequest
                    {
                         Action = "RagFairAddOffer",
                         sellInOnePiece = sellAll,
                         items = items,
                         requirements = new List<SellRequirement>{
                             new SellRequirement
                              {
                                 _tpl = requirement._tpl,
                                 count = requirement.count,
                                 level = 0,
                                 side = 0,
                                 onlyFunctional= false
                              }
                         }
                    }
                },
                tm = 2
            };
            string response = await PostJson<string>($"{PROD_ENDPOINT}/client/game/profile/items/moving", JsonConvert.SerializeObject(body));
            Logger.Log($"Sell:\n {response}", LoggingLevel.Verbose);
        }
    }    
}
