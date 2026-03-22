using System.Collections.Immutable;
using System.Threading;
using FXExchange.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Providers;

public sealed class RateProvider
{
    private ImmutableDictionary<string, decimal> _rates;

    private readonly object _writeLock = new();

    private readonly ILogger<RateProvider> _logger;

    public DateTime LastUpdated { get; private set; }

    public int Version { get; private set; }

    public RateProvider(
        ILogger<RateProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;

        _rates = Seed();

        LastUpdated = DateTime.UtcNow;

        Version = 1;
    }

    /// <summary>
    /// Seeds initial FX rates.
    /// Immutable dictionary guarantees safe concurrent reads.
    /// </summary>
    private static ImmutableDictionary<string, decimal> Seed()
    {
        return new Dictionary<string, decimal>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["EUR"] = 743.94m,
            ["USD"] = 663.11m,
            ["GBP"] = 852.85m,
            ["SEK"] = 76.10m,
            ["NOK"] = 78.40m,
            ["CHF"] = 683.58m,
            ["JPY"] = 5.9740m,
            ["DKK"] = 100m

        }.ToImmutableDictionary(
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Lock-free read using snapshot pattern.
    /// </summary>
    public decimal Get(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException(
                "Currency is required",
                "FX_CURRENCY_REQUIRED");
        }

        currency =
            currency.Trim()
            .ToUpperInvariant();

        var snapshot =
            Volatile.Read(ref _rates);

        if (!snapshot.TryGetValue(
            currency,
            out var rate))
        {
            _logger.LogWarning(
            "Currency not found {Currency}",
            currency);

            throw new DomainException(
                $"Currency {currency} not supported",
                "FX_UNKNOWN_CURRENCY");
        }

        if (rate <= 0)
        {
            _logger.LogError(
            "Invalid cached rate {Currency} {Rate}",
            currency,
            rate);

            throw new DomainException(
                "Invalid rate detected",
                "FX_INVALID_RATE");
        }

        return rate;
    }

    /// <summary>
    /// Returns current immutable snapshot.
    /// Safe for monitoring and diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, decimal>
    GetSnapshot()
    {
        return Volatile.Read(ref _rates);
    }

    /// <summary>
    /// Atomic snapshot update.
    /// Writers lock briefly.
    /// Readers remain lock-free.
    /// </summary>
    public void UpdateSnapshot(
        ImmutableDictionary<string, decimal>
        newRates)
    {
        if (newRates is null)
        {
            throw new DomainException(
                "Rates cannot be null",
                "FX_RATES_NULL");
        }

        if (newRates.Count == 0)
        {
            throw new DomainException(
                "Rates cannot be empty",
                "FX_RATES_EMPTY");
        }

        ValidateRates(newRates);

        lock (_writeLock)
        {
            Interlocked.Exchange(
                ref _rates,
                newRates);

            Version++;

            LastUpdated =
                DateTime.UtcNow;
        }

        _logger.LogInformation(
        "Rates updated Version {Version} Count {Count}",
        Version,
        newRates.Count);
    }

    /// <summary>
    /// Validates snapshot integrity.
    /// Prevents corrupt data entering cache.
    /// </summary>
    private void ValidateRates(
        ImmutableDictionary<string, decimal>
        rates)
    {
        foreach (var rate in rates)
        {
            if (string.IsNullOrWhiteSpace(
                rate.Key))
            {
                throw new DomainException(
                    "Currency code invalid",
                    "FX_INVALID_CURRENCY");
            }

            if (rate.Value <= 0)
            {
                _logger.LogError(
                "Invalid rate detected {Currency} {Rate}",
                rate.Key,
                rate.Value);

                throw new DomainException(
                    $"Invalid rate for {rate.Key}",
                    "FX_INVALID_RATE");
            }
        }
    }
}