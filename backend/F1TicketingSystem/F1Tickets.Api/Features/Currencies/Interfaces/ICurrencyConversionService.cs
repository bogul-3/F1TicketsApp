namespace F1Tickets.Api.Features.Currencies.Interfaces
{
    public interface ICurrencyConversionService
    {
        Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
    }
}
