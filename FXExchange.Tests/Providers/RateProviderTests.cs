using FXExchange.Infrastructure.Providers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace FXExchange.Tests.Providers;

public class RateProviderTests
{
    private readonly RateProvider _provider;

    public RateProviderTests()
    {
        var logger =
            new Mock<ILogger<RateProvider>>();

        _provider =
            new RateProvider(logger.Object);
    }

    [Fact]
    public void ShouldReturnRate()
    {
        _provider.Get("EUR")
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public void ShouldHandleLowerCase()
    {
        _provider.Get("eur")
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public void ShouldHandleTrim()
    {
        _provider.Get(" EUR ")
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public void ShouldThrowUnknown()
    {
        Action act =
            () => _provider.Get("XXX");

        act.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowNull()
    {
        Action act =
            () => _provider.Get(null);

        act.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldUpdateRates()
    {
        var rates =
        new Dictionary<string, decimal>
        {
            ["EUR"] = 999
        }.ToImmutableDictionary();

        _provider.UpdateSnapshot(rates);

        _provider.Get("EUR")
            .Should()
            .Be(999);
    }

    [Fact]
    public void ShouldIncreaseVersion()
    {
        var v = _provider.Version;

        _provider.UpdateSnapshot(
        new Dictionary<string, decimal>
        {
            ["EUR"] = 500
        }.ToImmutableDictionary());

        _provider.Version
            .Should()
            .Be(v + 1);
    }

    [Fact]
    public void ShouldUpdateTimestamp()
    {
        var t = _provider.LastUpdated;

        Thread.Sleep(10);

        _provider.UpdateSnapshot(
        new Dictionary<string, decimal>
        {
            ["EUR"] = 600
        }.ToImmutableDictionary());

        _provider.LastUpdated
            .Should()
            .BeAfter(t);
    }

    [Fact]
    public void ShouldHandleParallelReads()
    {
        Parallel.For(0, 1000, i =>
        {
            _provider.Get("EUR");
        });
    }

    [Fact]
    public void ShouldHandleParallelWrites()
    {
        Parallel.For(0, 100, i =>
        {
            _provider.UpdateSnapshot(
            new Dictionary<string, decimal>
            {
                ["EUR"] = 700 + i
            }.ToImmutableDictionary());
        });

        _provider.Get("EUR")
            .Should()
            .BeGreaterThan(0);
    }
}