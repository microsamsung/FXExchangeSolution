using FXExchange.Application.Interfaces;
using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Services;

public sealed class CurrencyService : ICurrencyService
{
    private readonly RateProvider _provider;

    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService( RateProvider provider, ILogger<CurrencyService> logger)
    {
        ArgumentNullException.ThrowIfNull(provider);

        ArgumentNullException.ThrowIfNull(logger);

        _provider = provider;

        _logger = logger;
    }

    public ValueTask<decimal> Convert( 
        string baseCurrency,
        string quoteCurrency,
        decimal amount)
    {
        try
        {
            // Input validation
            ArgumentException.ThrowIfNullOrWhiteSpace(baseCurrency);

            ArgumentException.ThrowIfNullOrWhiteSpace(quoteCurrency);

            if (amount <= 0)
            {
                _logger.LogWarning(
                "Invalid amount received {Amount}",
                amount);

                throw new ArgumentException(
                    "Amount must be positive");
            }

            // Normalize inputs
            baseCurrency = baseCurrency.Trim().ToUpperInvariant();

            quoteCurrency = quoteCurrency.Trim().ToUpperInvariant();

            _logger.LogInformation("Conversion requested {BaseCurrency} to {QuoteCurrency} Amount {Amount}",
                                    baseCurrency,
                                    quoteCurrency,
                                    amount);

            // Same currency fast path
            if (baseCurrency == quoteCurrency)
            {
                _logger.LogInformation(
                "Same currency conversion detected");

                return ValueTask.FromResult(amount);
            }

            // Get rates
            var baseRate = _provider.Get(baseCurrency);

            var quoteRate = _provider.Get(quoteCurrency);

            if (baseRate <= 0 || quoteRate <= 0)
            {
                _logger.LogError( "Invalid rate detected Base:{BaseRate} Quote:{QuoteRate}", baseRate,quoteRate);

                throw new InvalidOperationException("Invalid exchange rate");
            }

            _logger.LogInformation("Rates retrieved Base:{BaseRate} Quote:{QuoteRate}",baseRate,quoteRate);

            // Conversion calculation
            var conversionRate = quoteRate / baseRate;

            var result = amount * conversionRate;

            _logger.LogInformation( "Conversion completed Rate:{Rate} Result:{Result}", conversionRate,result);

            return ValueTask.FromResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning( ex, "Validation failure during conversion");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError( ex, "Unexpected conversion failure {BaseCurrency} {QuoteCurrency}", baseCurrency, quoteCurrency);

            throw;
        }
    }
}