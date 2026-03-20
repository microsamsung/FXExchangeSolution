using FXExchange.Application.Interfaces;
using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Services;

public sealed class CurrencyService : ICurrencyService
{
    private readonly RateProvider _provider;

    private readonly ILogger<CurrencyService>
        _logger;

    public CurrencyService(
        RateProvider provider,
        ILogger<CurrencyService> logger)
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
            ArgumentException
                .ThrowIfNullOrWhiteSpace(baseCurrency);

            ArgumentException
                .ThrowIfNullOrWhiteSpace(quoteCurrency);

            if (amount <= 0)
                throw new ArgumentException(
                    "Amount must be positive");
            // Normalize inputs
            baseCurrency =
                baseCurrency.Trim()
                .ToUpperInvariant();

            quoteCurrency =
                quoteCurrency.Trim()
                .ToUpperInvariant();

            _logger.LogInformation(
            "Conversion {Base} {Quote} {Amount}",
            baseCurrency,
            quoteCurrency,
            amount);

            // Same currency fast path
            if (baseCurrency == quoteCurrency)
                return ValueTask.FromResult(amount);
            // Get rates
            var baseRate =
                _provider.Get(baseCurrency);
            // Conversion calculation
            var quoteRate =
                _provider.Get(quoteCurrency);

            var result =
                amount *
                (quoteRate / baseRate);

            _logger.LogInformation(
            "Conversion result {Result}",
            result);

            return ValueTask.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Conversion failed");

            throw;
        }
    }
}