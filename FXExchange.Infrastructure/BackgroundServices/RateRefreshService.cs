using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace FXExchange.Infrastructure.BackgroundServices;

public sealed class RateRefreshService : BackgroundService
{
    private readonly RateProvider _provider;

    private readonly ILogger<RateRefreshService> _logger;

    public RateRefreshService( RateProvider provider, ILogger<RateRefreshService> logger)
    {
        _provider = provider;

        _logger = logger;
    }

    protected override async Task ExecuteAsync( CancellationToken stoppingToken)
    {
        _logger.LogInformation(
        "Rate refresh service started");

        while (!stoppingToken
            .IsCancellationRequested)
        {
            try
            {
                // Simulate market update
                var newRates =
                    GenerateRates();

                _provider.UpdateSnapshot( newRates);

                _logger.LogInformation( "Rates refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError( ex, "Rate refresh failed");
            }

            await Task.Delay( TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private ImmutableDictionary<string, decimal> GenerateRates()
    {
        var random = new Random();

        return new Dictionary<string, decimal>
        {
            ["EUR"] = 743.94m +
            random.Next(-5, 5),

            ["USD"] = 663.11m +
            random.Next(-5, 5),

            ["GBP"] = 852.85m +
            random.Next(-5, 5),

            ["SEK"] = 76.10m,

            ["NOK"] = 78.40m,

            ["CHF"] = 683.58m,

            ["JPY"] = 5.9740m,

            ["DKK"] = 100m

        }.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
    }
}