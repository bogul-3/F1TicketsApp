using F1Tickets.Api.Features.Currencies.Interfaces;
using System.Text.Json;

namespace F1Tickets.Api.Features.Currencies.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly HttpClient _http;

        public CurrencyConversionService(HttpClient http)
        {
            _http = http;
        }
        public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            // 1 Check if currency is the same
            if (fromCurrency == toCurrency) return amount;

            // 2 Get response from API
            var url = $"https://v6.exchangerate-api.com/v6/174261af5b4e05b1346a247a/latest/{fromCurrency}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            // 3 Check if API response is successful
            if (json.GetProperty("result").GetString() != "success")
                throw new Exception("Currency API returned an error");

            // 4 Get conversion rates from response
            var rates = json.GetProperty("conversion_rates");
            if (!rates.TryGetProperty(toCurrency, out var rateElement))
                throw new Exception($"Currency {toCurrency} not found in API response");

            // 5 Calculate price
            var rate = rateElement.GetDecimal();
            return amount * rate;
        }
    }
}
