using FluentAssertions;
using FXExchange.Application.Commands;
using FXExchange.Infrastructure.Services;
using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Handlers;

public class ConvertCurrencyHandlerTests
{
    private readonly ConvertCurrencyHandler _handler;

    public ConvertCurrencyHandlerTests()
    {
        var rlog =
            new Mock<ILogger<RateProvider>>();

        var slog =
            new Mock<ILogger<CurrencyService>>();

        var hlog =
            new Mock<ILogger<ConvertCurrencyHandler>>();

        var provider =
            new RateProvider(rlog.Object);

        var service =
            new CurrencyService(provider, slog.Object);

        _handler =
            new ConvertCurrencyHandler(service, hlog.Object);
    }

    [Fact]
    public async Task ShouldReturnSuccess()
    {
        var cmd = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "USD",
            Amount = 10
        };

        var r =
        await _handler.Handle(
            cmd,
            CancellationToken.None);

        r.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnFailure()
    {
        var cmd = new ConvertCurrencyCommand
        {
            BaseCurrency = "XXX",
            QuoteCurrency = "USD",
            Amount = 10
        };

        var r =
        await _handler.Handle(
            cmd,
            CancellationToken.None);

        r.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldHandleSameCurrency()
    {
        var cmd = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "EUR",
            Amount = 10
        };

        var r =
        await _handler.Handle(
            cmd,
            CancellationToken.None);

        r.Value.Should().Be(10);
    }
}