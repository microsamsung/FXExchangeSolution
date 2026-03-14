namespace FXExchange.Application.Interfaces;

public interface ICurrencyService
{
    ValueTask<decimal> Convert(
        string baseCurrency,
        string quoteCurrency,
        decimal amount);
}