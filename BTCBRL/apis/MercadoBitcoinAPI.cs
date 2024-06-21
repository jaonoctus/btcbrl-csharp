using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BTCBRL.Interfaces;

namespace BTCBRL.APIs
{
    public class MercadoBitcoinAPIResponse
    {
        public string last { get; set; }
    }

    public class MercadoBitcoinAPI: IPriceAPI
    {
        public async Task<decimal> GetPriceAsync()
        {
            using var client = new HttpClient();
            var requestUrl = "https://api.mercadobitcoin.net/api/v4/tickers?symbols=BTC-BRL";
            var jsonString = await client.GetStringAsync(requestUrl);
            var response = JsonSerializer.Deserialize<MercadoBitcoinAPIResponse[]>(jsonString);

            if (response == null)
            {
                throw new Exception("Failed to deserialize response.");
            };

            return decimal.Round(decimal.Parse(response[0].last.Replace(".", ",")), 2, MidpointRounding.AwayFromZero);
        }
    }
}