using System.Collections.Concurrent;
using System.Collections.Immutable;
using FluentAssertions;
using FXExchange.Domain.Exceptions;
using FXExchange.Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;

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
    public void Get_WithKnownCurrency_ReturnsExpectedRate()
    {
        _provider.Get("EUR")
            .Should()
            .Be(743.94m);
    }

    [Fact]
    public void Get_WithLowerCaseCurrency_ReturnsExpectedRate()
    {
        _provider.Get("eur")
            .Should()
            .Be(743.94m);
    }

    [Fact]
    public void Get_WithWhitespaceCurrency_ReturnsExpectedRate()
    {
        _provider.Get(" EUR ")
            .Should()
            .Be(743.94m);
    }

    [Fact]
    public void Get_WithUnknownCurrency_ThrowsDomainException()
    {
        Action act =
            () => _provider.Get("XXX");

        act.Should()
            .Throw<DomainException>()
            .WithMessage("*not supported*");
    }

    [Fact]
    public void Get_WithNullCurrency_ThrowsDomainException()
    {
        Action act =
            () => _provider.Get(null!);

        act.Should()
            .Throw<DomainException>()
            .WithMessage("*Currency is required*");
    }

    [Fact]
    public void UpdateSnapshot_ReplacesExistingRate()
    {
        var rates =
            new Dictionary<string, decimal>
            {
                ["EUR"] = 999m
            }
            .ToImmutableDictionary();

        _provider.UpdateSnapshot(rates);

        _provider.Get("EUR")
            .Should()
            .Be(999m);
    }

    [Fact]
    public void UpdateSnapshot_IncrementsVersion()
    {
        var previousVersion =
            _provider.Version;

        _provider.UpdateSnapshot(
            new Dictionary<string, decimal>
            {
                ["EUR"] = 500m
            }
            .ToImmutableDictionary());

        _provider.Version
            .Should()
            .Be(previousVersion + 1);
    }

    [Fact]
    public void UpdateSnapshot_UpdatesTimestamp()
    {
        var previousTimestamp =
            _provider.LastUpdated;

        Thread.Sleep(20);

        _provider.UpdateSnapshot(
            new Dictionary<string, decimal>
            {
                ["EUR"] = 600m
            }
            .ToImmutableDictionary());

        _provider.LastUpdated
            .Should()
            .BeAfter(previousTimestamp);
    }

    [Fact]
    public void UpdateSnapshot_WithNullRates_ThrowsDomainException()
    {
        Action act =
            () => _provider.UpdateSnapshot(null!);

        act.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void UpdateSnapshot_WithEmptyRates_ThrowsDomainException()
    {
        Action act =
            () => _provider.UpdateSnapshot(
                ImmutableDictionary<string, decimal>.Empty);

        act.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void UpdateSnapshot_WithInvalidRate_ThrowsDomainException()
    {
        var rates =
            new Dictionary<string, decimal>
            {
                ["EUR"] = -1m
            }
            .ToImmutableDictionary();

        Action act =
            () => _provider.UpdateSnapshot(rates);

        act.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void GetSnapshot_ReturnsSnapshot()
    {
        var snapshot =
            _provider.GetSnapshot();

        snapshot.Should()
            .NotBeNull();

        snapshot.Should()
            .ContainKey("EUR");

        snapshot["EUR"]
            .Should()
            .Be(743.94m);
    }

    [Fact]
    public void ParallelReads_DoNotThrowExceptions()
    {
        var exceptions =
            new ConcurrentBag<Exception>();

        Parallel.For(0, 1000, i =>
        {
            try
            {
                _provider.Get("EUR");
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        exceptions.Should().BeEmpty();
    }

    [Fact]
    public void ParallelWrites_DoNotCorruptState()
    {
        var exceptions =
            new ConcurrentBag<Exception>();

        Parallel.For(0, 100, i =>
        {
            try
            {
                _provider.UpdateSnapshot(
                    new Dictionary<string, decimal>
                    {
                        ["EUR"] = 700m + i
                    }
                    .ToImmutableDictionary());
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        exceptions.Should().BeEmpty();

        _provider.Get("EUR")
            .Should()
            .BeInRange(
                700m,
                799m);
    }
}