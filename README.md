# TarkovBot

Hey, this is next EFT application that can communicate with the market API,

Credits:
> https://github.com/dank/tarkov

Small example to get you started
```cs
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
            itemsDictionary.Add("energy_lamp", "590a3cd386f77436f20848cb");

        }

 public async Task StartAsync()
        {
            playerData = await tarkovHttpClient.Auth();
            if (playerData == null)
                return;

            var search = await tarkovHttpClient.SearchFleaMarket(itemsDictionary["energy_lamp"]);
            var cheapestOffer = search.data.offers.First();
            // find big enough money stack
            var cash = playerData.Inventory.items.First(x => x._tpl == itemsDictionary["rouble"] && (ulong)x.upd?.StackObjectsCount >= cheapestOffer.summaryCost);
            await Task.Delay(10);

            // get first offer from search, they should be sorted by price (so far based on the results from API) and buy it
            var buyStatus = await tarkovHttpClient.BuyItem(cheapestOffer._id, 1, new List<BarterItem> { new BarterItem { count = cheapestOffer.summaryCost, id = cash._id} });
           
            if (buyStatus == BuyStatus.OfferNotFound) // if you actually bought it fast enough, (noone bought it before you)
            {
                Console.WriteLine("Someone was faster");
                return;
            }
            // refresh inventory so you can see it
            playerData = await tarkovHttpClient.RefreshProfile();
            // now to sell this type of item for the same price as bought
            var item = playerData.Inventory.items.FirstOrDefault(x => x._tpl == itemsDictionary["energy_lamp"]);
            var sellResult = await tarkovHttpClient.SellItem(new List<string> { item._id}, false, new Requirement { count = cheapestOffer.summaryCost, _tpl = itemsDictionary["rouble"] });

            await tarkovHttpClient.KeepAlive();
        }
```
