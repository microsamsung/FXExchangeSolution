using FXExchange.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FXExchange.Infrastructure.Providers;

public sealed class RateRefreshService
    : BackgroundService
{
    private readonly IRateSource _rateSource;

    private readonly IRateProvider _provider;

    private readonly ILogger<RateRefreshService>
        _logger;

    public RateRefreshService(
        IRateSource rateSource,
        IRateProvider provider,
        ILogger<RateRefreshService> logger)
    {
        ArgumentNullException.ThrowIfNull(rateSource);
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(logger);

        _rateSource = rateSource;
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Rate refresh service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var rates =
                    await _rateSource.GetRatesAsync(
                        stoppingToken);

                _provider.UpdateSnapshot(
                    rates);

                _logger.LogInformation(
                    "Rates refreshed. Version {Version}",
                    _provider.Version);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to refresh exchange rates");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(10),
                stoppingToken);
        }

        _logger.LogInformation(
            "Rate refresh service stopped");
    }
}