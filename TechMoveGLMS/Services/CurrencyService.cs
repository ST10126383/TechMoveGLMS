using System.Text.Json;


namespace TechMoveGLMS.Services
{
   
        public class CurrencyService : ICurrencyService
        {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
        {
            if (usdAmount <= 0) return 0;

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(
                    "https://v6.exchangerate-api.com/v6/test/pair/USD/ZAR");   // Changed for testing

                if (response?.ConversionRate > 0)
                {
                    return Math.Round(usdAmount * response.ConversionRate, 2);
                }
            }
            catch { }

            return Math.Round(usdAmount * 18.20m, 2); // Fallback
        }
    }

    public class ExchangeRateResponse
    {
        public decimal ConversionRate { get; set; }
    }
}

