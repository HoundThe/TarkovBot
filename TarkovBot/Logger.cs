using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovBot
{
    public enum LoggingLevel
    {
        Verbose,
        Medium,
        Low
    }
    public static class Logger
    {

        public static void Log(string content, LoggingLevel verbosity)
        {
            Console.WriteLine($"[{verbosity.ToString()}]: {content}");
        }

        public static void Log(SearchResponse search, LoggingLevel verbosity)
        {
            Console.WriteLine("List of offers:");
            int i = 0;
            foreach (var offer in search.data.offers)
            {
                Console.WriteLine($"Offer {++i} Id: {offer._id}");
                Console.WriteLine($"User: {offer.user.nickname}, with rating: {offer.user.rating}");
                Console.WriteLine($"His summ/req/item cost {offer.summaryCost}/{offer.requirementCost}/{offer.itemCost}");
                Console.WriteLine($"Start time: {DateTimeOffset.FromUnixTimeSeconds(offer.startTime)} EndTime: {DateTimeOffset.FromUnixTimeSeconds(offer.endTime)}");
                Console.WriteLine($"Timestamp difference {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - offer.startTime}");
            }
        }

        public static void Log(Inventory inv, LoggingLevel verbosity)
        {
            Console.WriteLine("Inventory listing:");
            foreach(var item in inv.items)
            {
                Console.WriteLine($"Item id: {item._id}, item type id: {item._tpl}, Quantity: {item.upd?.StackObjectsCount}");
            }
        }
    }
}
