using System.Globalization;
using BTCBRL.APIs;
using BTCBRL.Interfaces;

namespace BTCBRL
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the currency amount in BRL in the first argument.");
                Environment.Exit(1);
            }

            if (args.Length == 1)
            {
                Console.WriteLine("Please provide the premium (%) in the second argument.");
                Environment.Exit(1);
            }

            var currencyAmountInput = args[0];
            var premiumInput = args[1];

            var price = await GetQuoteAsync();

            var btcbrlPremium = price * ((100 + decimal.Parse(premiumInput)) / 100);
            var btcSellAmount = decimal.Parse(currencyAmountInput) / btcbrlPremium;
            var satsSellAmount = btcSellAmount * 100_000_000;

            var format = CultureInfo.GetCultureInfo("pt-BR");
            Console.WriteLine(string.Format(format, "Quote (BTC/BRL): {0:C}", price));
            Console.WriteLine($"Seller premium: {premiumInput}%");
            Console.WriteLine(string.Format(format, "Quote with premium (BTC/BRL): {0:C}", btcbrlPremium));
            Console.WriteLine(string.Format(format, "Buy offer (BRL): {0:C}", decimal.Parse(currencyAmountInput)));
            Console.WriteLine("--------------------");
            Console.WriteLine(string.Format(format, "Sell offer (BTC): {0:0.00000000}", btcSellAmount));
            Console.WriteLine(string.Format(format, "Sell offer (Sats): {0:N0}", satsSellAmount));
        }

        private static async Task<decimal> GetQuoteAsync()
        {
            List<IPriceAPI> apis = [
                new BrasilBitcoinAPI(),
                new MercadoBitcoinAPI()
            ];

            var semaphore = new SemaphoreSlim(5);

            var tasks = apis.Select(async api =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await api.GetPriceAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var prices = await Task.WhenAll(tasks);

            var averagePrice = prices.Average();

            return averagePrice;
        }
    }
}
