using FXExchange.Application.Interfaces;
using FXExchange.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Services;

public sealed class CurrencyService : ICurrencyService
{
    private readonly IRateProvider _provider;

    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(
        IRateProvider provider,
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
            if (string.IsNullOrWhiteSpace(baseCurrency))
            {
                throw new DomainException(
                    "Base currency is required",
                    "FX_BASE_REQUIRED");
            }

            if (string.IsNullOrWhiteSpace(quoteCurrency))
            {
                throw new DomainException(
                    "Quote currency is required",
                    "FX_QUOTE_REQUIRED");
            }

            if (amount <= 0)
            {
                throw new DomainException(
                    "Amount must be positive",
                    "FX_INVALID_AMOUNT");
            }

            baseCurrency =
                baseCurrency.Trim()
                    .ToUpperInvariant();

            quoteCurrency =
                quoteCurrency.Trim()
                    .ToUpperInvariant();

            if (baseCurrency == quoteCurrency)
            {
                return ValueTask.FromResult(amount);
            }

            var baseRate =
                _provider.Get(baseCurrency);

            var quoteRate =
                _provider.Get(quoteCurrency);

            if (baseRate <= 0 )
            {
                throw new DomainException(
                    "Invalid exchange rate",
                    "FX_INVALID_RATE");
            }

            // FIXED FORMULA
            var result =
                amount * (baseRate / quoteRate);

            return ValueTask.FromResult(result);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Conversion failed");

            throw new DomainException(
                "Conversion failed",
                "FX_CONVERSION_ERROR",
                ex);
        }
    }
}