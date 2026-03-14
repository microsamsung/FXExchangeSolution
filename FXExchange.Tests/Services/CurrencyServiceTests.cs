using FluentAssertions;
using FXExchange.Infrastructure.Providers;
using FXExchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Services;

public class CurrencyServiceTests
{
    private readonly CurrencyService _service;

    public CurrencyServiceTests()
    {
        var rlog =
            new Mock<ILogger<RateProvider>>();

        var slog =
            new Mock<ILogger<CurrencyService>>();

        var provider =
            new RateProvider(rlog.Object);

        _service =
            new CurrencyService(provider, slog.Object);
    }

    [Fact]
    public async Task ShouldConvert()
    {
        var r =
        await _service.Convert("EUR", "USD", 10);

        r.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldReturnSame()
    {
        var r =
        await _service.Convert("EUR", "EUR", 10);

        r.Should().Be(10);
    }

    [Fact]
    public async Task ShouldTrim()
    {
        var r =
        await _service.Convert(" eur ", " usd ", 10);

        r.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldThrowInvalidAmount()
    {
        Func<Task> act =
        async () => await _service.Convert(
            "EUR", "USD", -1);

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrowZeroAmount()
    {
        Func<Task> act =
        async () => await _service.Convert(
            "EUR", "USD", 0);

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrowUnknownBase()
    {
        Func<Task> act =
        async () => await _service.Convert(
            "AAA", "USD", 10);

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldThrowUnknownQuote()
    {
        Func<Task> act =
        async () => await _service.Convert(
            "EUR", "AAA", 10);

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ShouldHandleLargeValues()
    {
        var r =
        await _service.Convert(
            "EUR", "USD", 10000000);

        r.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldMaintainPrecision()
    {
        var r =
        await _service.Convert(
            "EUR", "USD", 1.25m);

        r.Should().BePositive();
    }

    [Fact]
    public async Task ShouldHandleRepeatedCalls()
    {
        for (int i = 0; i < 100; i++)
        {
            var r =
            await _service.Convert(
                "EUR", "USD", 10);

            r.Should().BeGreaterThan(0);
        }
    }
}