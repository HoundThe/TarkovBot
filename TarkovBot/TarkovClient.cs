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
        private const int _heartbeatDelay = 25000;
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

            if ((int)response.StatusCode != 200)
            {
                Console.WriteLine("Status code: " + (int)response.StatusCode);
                return default(T);
            }

            string decodedData;

            try
            {
            decodedData = await DeflateResponse(data);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return default(T);
            }
            // string type will just return the text of response
            if (typeof(T) == typeof(string))
                return (T)(object)decodedData;
            else
                return JsonConvert.DeserializeObject<T>(decodedData);
        }

        public async Task<PlayerData> Auth()
        {
            var playerData = await RefreshProfile();
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
                Logger.Log("Heartbeat", LoggingLevel.Verbose);
                await PostJson<string>(PROD_ENDPOINT + "/client/game/keepalive", "");
                await Task.Delay(_heartbeatDelay);
            }
        }
        public async Task<PlayerData> RefreshProfile()
        {
            var profile = await PostJson<Profile>(PROD_ENDPOINT + "/client/game/profile/list", "");
            if (profile == default(Profile))
            {
                Logger.Log("Servers are fucked again", LoggingLevel.Low);
                return null;
            }
            var playerData = profile.data?.FirstOrDefault(x => x.Info.Side != "Savage");

            if (playerData == null)
            {
                Logger.Log("[REFRESH] Player data not found, authentication failed, invalid session?", LoggingLevel.Low);
                return null;
            }

            return playerData;
        }

        public async Task<SearchResponse> SearchFleaMarket(string itemId)
        {
            var request = new FleaRequest
            {
                page = 0,
                limit = 15,
                sortType = SortBy.Price,
                sortDirection = SortDirection.Ascending,
                currency = Currency.Rouble,
                priceFrom = 0,
                priceTo = 0,
                quantityFrom = 0,
                quantityTo = 0,
                conditionFrom = 0,
                conditionTo = 0,
                oneHourExpiration = false,
                removeBartering = true,
                offerOwnerType = Owner.Player,
                onlyFunctional = true,
                updateOfferCount = true,
                handbookId = itemId,
                linkedSearchId = "",
                neededSearchId = "",
                tm = 1,

            };
            var response = await PostJson<SearchResponse>($"{TarkovHttpClient.RAGFAIR_ENDPOINT}/client/ragfair/find", JsonConvert.SerializeObject(request));
            if (response == default(SearchResponse))
            {
                Logger.Log("Servers are fucked again", LoggingLevel.Low);
                return null;
            }

            return response;
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
            // gotta parse it myself because the api is inconsistent
            var stringResponse = await PostJson<string>($"{PROD_ENDPOINT}/client/game/profile/items/moving", JsonConvert.SerializeObject(body));

            return HandleBuyErrors(stringResponse);

        }

        private BuyStatus HandleBuyErrors(string response)
        {
            if (response == default(string))
            {
                Logger.Log("Servers are fucked again", LoggingLevel.Low);
                return BuyStatus.BackendError;
            }

            if (!response.Contains("\"err\":0"))
            {
                if (response.Contains("has count")) // not big enough money stack
                    return BuyStatus.NotEnoughMoney;
                if (response.Contains("enough money")) // not enough money to pay the tax
                    return BuyStatus.NotEnoughMoney;
                if (response.Contains("not found"))
                    return BuyStatus.OfferNotFound;
                if (response.Contains("filed to lock profile"))
                    return BuyStatus.ProfileLocked;

                Console.Write("[BUY] ERROR - " + response);
                return BuyStatus.OtherError;
            }


            else
            {
                var parsedResponse = JsonConvert.DeserializeObject<BuyItemResponse>(response);

                if (parsedResponse.data.badRequest.Count != 0)
                {
                    var errorMessage = parsedResponse.data.badRequest.First().errmsg;
                //    Logger.Log($"[BUY]: {errorMessage}", LoggingLevel.Verbose);
                    if (errorMessage.Contains("not found"))
                        return BuyStatus.OfferNotFound;
                    if (errorMessage.Contains("place"))
                        return BuyStatus.InventoryFull;
                    if (errorMessage.Contains("locked"))
                        return BuyStatus.ProfileLocked;

                    Console.Write("[BUY] ERROR - " + response);

                    return BuyStatus.OtherError;
                }
                else
                {
                    return BuyStatus.Success;
                }
            }
        }
        public async Task GetMoneyFromMessage(Inventory inventory)
        {
            int marketType = 4; // market
            var response = await PostJson<MessageResponse>($"{PROD_ENDPOINT}/client/mail/dialog/list", "");
            if (response == default(MessageResponse))
            {
                Logger.Log("Servers are fucked again", LoggingLevel.Low);
                return;
            }
            var x = response.data.FirstOrDefault(x => x.type == marketType);
            if (x == null)
                return;
            await Task.Delay(500);
            // get all attachments
            var body = $"{{\"dialogId\":\"{x._id}\"}}";
            var resp = await PostJson<AttachmentsResponse>($"{PROD_ENDPOINT}/client/mail/dialog/getAllAttachments", body);
            if (resp == default(AttachmentsResponse))
            {
                Logger.Log("Servers are fucked again", LoggingLevel.Low);
                return;
            }
            foreach (var mes in resp.data.messages)
            {
                await Task.Delay(100);

                body = $"{{" +
              $"\"data\": [{{" +
              $"\"Action\":\"Move\"," +
              $"\"item\":\"{mes.items.data.First()._id}\"," +
              $"\"to\":{{" +
              $"\"id\":\"{inventory.stash}\"," +
              $"\"container\":\"hideout\"" +
              $"}}," +
              $"\"fromOwner\":{{" +
              $"\"id\":\"{mes._id}\"," +
              $"\"type\":\"Mail\"" +
              $"}}" +
              $"}}]" +
              $"}}";

                var rp = await PostJson<Response>($"{PROD_ENDPOINT}/client/game/profile/items/moving", body);
                if (rp == default(Response))
                {
                    Logger.Log("Servers are fucked again", LoggingLevel.Low);
                    return;
                }
                if (response.errmsg != null)
                {
                    Console.WriteLine(response.errmsg);
                    Console.WriteLine("Inventory probably full, returning");
                    break;
                }
            }
        }

        public async Task StackAllCash(Inventory inventory)
        {
            string roubleId = "5449016a4bdc2d6f028b456f";

            var cashIds = inventory.items.Where(x => x._tpl == roubleId).ToList();

            Item fromItem;
            Item toItem;
            while (true)
            {
                await Task.Delay(200);
                fromItem = cashIds.FirstOrDefault();
                cashIds.Remove(fromItem);
                if (fromItem == null)
                    break;
                toItem = cashIds.FirstOrDefault(x => x?.upd?.StackObjectsCount + fromItem.upd.StackObjectsCount < 500000);
                if (toItem == null)
                    continue;
                toItem.upd.StackObjectsCount += fromItem.upd.StackObjectsCount; // update the value

                // do the move
                var body = $"{{\"data\":[{{" +
              $"\"Action\":\"Merge\"," +
              $"\"item\":\"{fromItem._id}\"," +
              $"\"with\":\"{toItem._id}\"" +
              $"}}]," +
              $"\"tm\":2" +
              $"}}";


                var response = await PostJson<Response>($"{PROD_ENDPOINT}/client/game/profile/items/moving", body);
                if (response == default(Response))
                {
                    return;
                }
                if (response.errmsg != null)
                {
                    Console.WriteLine(response.errmsg);
                }
            }
        }



        public async Task<SellStatus> SellItem(List<string> items, bool sellAll, Requirement requirement)
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
            var response = await PostJson<Response>($"{PROD_ENDPOINT}/client/game/profile/items/moving", JsonConvert.SerializeObject(body));
            if(response == default(Response))
            {
                return SellStatus.BackendError;
            }
            if (response.err != 0)
            {
                Logger.Log($"[SELL] error {response.errmsg}", LoggingLevel.Verbose);
                if (response.errmsg.Contains("max offer"))
                    return SellStatus.NoAvailableOffer;
                if (response.errmsg.Contains("pay the tax"))
                    return SellStatus.NoMoneyForTax;
                return SellStatus.OtherErr;
            }
            else {
                Logger.Log($"[SELL]:\n {response.data.ToString()}", LoggingLevel.Verbose);
                return SellStatus.Success;
            }
        }
    }
}
