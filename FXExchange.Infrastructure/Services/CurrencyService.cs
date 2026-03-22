using FXExchange.Application.Interfaces;
using FXExchange.Domain.Exceptions;
using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Services;

public sealed class CurrencyService : ICurrencyService
{
    private readonly RateProvider _provider;

    private readonly ILogger<CurrencyService> _logger;

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
            // Validate inputs
            if (string.IsNullOrWhiteSpace(baseCurrency))
                throw new DomainException(
                    "Base currency is required",
                    "FX_BASE_REQUIRED");

            if (string.IsNullOrWhiteSpace(quoteCurrency))
                throw new DomainException(
                    "Quote currency is required",
                    "FX_QUOTE_REQUIRED");

            if (amount <= 0)
                throw new DomainException(
                    "Amount must be positive",
                    "FX_INVALID_AMOUNT");

            // Normalize
            baseCurrency =
                baseCurrency.Trim()
                .ToUpperInvariant();

            quoteCurrency =
                quoteCurrency.Trim()
                .ToUpperInvariant();

            _logger.LogInformation(
            "Starting conversion {Base} -> {Quote} Amount {Amount}",
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
            var baseRate =
                _provider.Get(baseCurrency);

            var quoteRate =
                _provider.Get(quoteCurrency);

            // Defensive validation
            if (baseRate <= 0 || quoteRate <= 0)
            {
                throw new DomainException(
                    "Invalid exchange rate",
                    "FX_INVALID_RATE");
            }

            // Conversion calculation
            var result =
                amount * (quoteRate / baseRate);

            _logger.LogInformation(
            "Conversion completed Rate:{Rate} Result:{Result}",
            quoteRate / baseRate,
            result);

            return ValueTask.FromResult(result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
            ex,
            "Business validation failed");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Unexpected conversion failure {Base} {Quote}",
            baseCurrency,
            quoteCurrency);

            throw new DomainException(
                "Conversion failed",
                "FX_CONVERSION_ERROR",
                ex);
        }
    }
}