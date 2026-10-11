namespace F1Tickets.Api.Features.Currencies.DTOs
{
    public class CurrencyDto
    {
        public string Code { get; set; } 
        public string Name { get; set; } 
        public decimal ExchangeRate { get; set; }
    }
}
