using FluentAssertions;
using FXExchange.Application.Interfaces;
using FXExchange.Domain.Exceptions;
using FXExchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Services;

public class CurrencyServiceTests
{
    private readonly CurrencyService _service;

    private readonly Mock<IRateProvider>
        _rateProvider;

    public CurrencyServiceTests()
    {
        _rateProvider =
            new Mock<IRateProvider>();

        var logger =
            new Mock<ILogger<CurrencyService>>();

        _service =
            new CurrencyService(
                _rateProvider.Object,
                logger.Object);
    }

    [Fact]
    public async Task Convert_EUR_To_USD_Returns_Expected_Value()
    {
        _rateProvider
            .Setup(x => x.Get("EUR"))
            .Returns(743.94m);

        _rateProvider
            .Setup(x => x.Get("USD"))
            .Returns(663.11m);

        var result =
            await _service.Convert(
                "EUR",
                "USD",
                10m);

        result.Should()
            .Be(11.218953114867819818732940236m);
    }

    [Fact]
    public async Task Convert_Same_Currency_Returns_Same_Amount()
    {
        var result =
            await _service.Convert(
                "EUR",
                "EUR",
                10m);

        result.Should()
            .Be(10m);
    }

    [Fact]
    public async Task Convert_Should_Trim_Currency_Codes()
    {
        _rateProvider
            .Setup(x => x.Get("EUR"))
            .Returns(743.94m);

        _rateProvider
            .Setup(x => x.Get("USD"))
            .Returns(663.11m);

        var result =
            await _service.Convert(
                " eur ",
                " usd ",
                10m);

        result.Should()
            .Be(11.218953114867819818732940236m);
    }

    [Fact]
    public async Task Convert_With_Negative_Amount_Should_Throw_DomainException()
    {
        Func<Task> act =
            async () =>
                await _service.Convert(
                    "EUR",
                    "USD",
                    -1m);

        await act.Should()
            .ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Convert_With_Zero_Amount_Should_Throw_DomainException()
    {
        Func<Task> act =
            async () =>
                await _service.Convert(
                    "EUR",
                    "USD",
                    0m);

        await act.Should()
            .ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Convert_With_Unknown_Base_Currency_Should_Throw_DomainException()
    {
        _rateProvider
            .Setup(x => x.Get("AAA"))
            .Throws(
                new DomainException(
                    "Currency AAA not supported",
                    "FX_UNKNOWN_CURRENCY"));

        Func<Task> act =
            async () =>
                await _service.Convert(
                    "AAA",
                    "USD",
                    10m);

        await act.Should()
            .ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Convert_With_Unknown_Quote_Currency_Should_Throw_DomainException()
    {
        _rateProvider
            .Setup(x => x.Get("AAA"))
            .Throws(
                new DomainException(
                    "Currency AAA not supported",
                    "FX_UNKNOWN_CURRENCY"));

        Func<Task> act =
            async () =>
                await _service.Convert(
                    "EUR",
                    "AAA",
                    10m);

        await act.Should()
            .ThrowAsync<DomainException>();
    }
}