namespace FXExchange.Domain.Entities;

/// <summary>
/// Represents exchange rate against DKK.
/// </summary>
public sealed class ExchangeRate
{
    public string Currency { get; private set; }

    public decimal Rate { get; private set; }

    private ExchangeRate()
    {
    }

    public ExchangeRate(string currency, decimal rate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        if (rate <= 0)
            throw new ArgumentException("Invalid rate");

        Currency = currency.ToUpperInvariant();

        Rate = rate;
    }

    public void UpdateRate(decimal rate)
    {
        if (rate <= 0)
            throw new ArgumentException("Invalid rate");

        Rate = rate;
    }
}