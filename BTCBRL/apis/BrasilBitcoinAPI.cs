using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BTCBRL.Interfaces;

namespace BTCBRL.APIs
{
    public class BrasilBitcoinAPIResponse
    {
        public string last { get; set; }
    }

    public class BrasilBitcoinAPI: IPriceAPI
    {
        public async Task<decimal> GetPriceAsync()
        {
            using var client = new HttpClient();
            var requestUrl = "https://brasilbitcoin.com.br/API/prices/BTC";
            var jsonString = await client.GetStringAsync(requestUrl);
            var response = JsonSerializer.Deserialize<BrasilBitcoinAPIResponse>(jsonString);

            if (response == null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return decimal.Round(decimal.Parse(response.last.Replace(".", ",")), 2, MidpointRounding.AwayFromZero);
        }
    }
}