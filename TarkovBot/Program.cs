using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace TarkovBot
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var marketBot = new Bot("Your session");
                await marketBot.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
