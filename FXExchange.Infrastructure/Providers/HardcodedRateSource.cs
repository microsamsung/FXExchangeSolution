using System.Collections.Immutable;
using FXExchange.Application.Interfaces;

namespace FXExchange.Infrastructure.Providers;

public sealed class HardcodedRateSource
    : IRateSource
{
    private static readonly Dictionary<string, decimal>
        BaseRates =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["EUR"] = 743.94m,
                ["USD"] = 663.11m,
                ["GBP"] = 852.85m,
                ["SEK"] = 76.10m,
                ["NOK"] = 78.40m,
                ["CHF"] = 683.58m,
                ["JPY"] = 5.9740m,
                ["DKK"] = 100m
            };

    public Task<ImmutableDictionary<string, decimal>>
        GetRatesAsync(
            CancellationToken cancellationToken)
    {
        var rates =
            BaseRates.ToDictionary(
                x => x.Key,
                x => ApplyMarketMovement(x.Value),
                StringComparer.OrdinalIgnoreCase);

        return Task.FromResult(
            rates.ToImmutableDictionary(
                StringComparer.OrdinalIgnoreCase));
    }

    private static decimal ApplyMarketMovement(
        decimal rate)
    {
        // Simulate ±0.5% market movement
        var percentageChange =
            (decimal)(Random.Shared.NextDouble() - 0.5)
            / 100m;

        var updatedRate =
            rate * (1 + percentageChange);

        return Math.Round(
            updatedRate,
            4,
            MidpointRounding.AwayFromZero);
    }
}