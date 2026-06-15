using FluentAssertions;
using FXExchange.Domain.Exceptions;
using FXExchange.Infrastructure.Providers;
using FXExchange.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Services;

public class CurrencyServiceTests
{
    private readonly CurrencyService _service;
    private readonly RateProvider _provider;

    public CurrencyServiceTests()
    {
        var rlog =
            new Mock<ILogger<RateProvider>>();

        var slog =
            new Mock<ILogger<CurrencyService>>();

        _provider =
            new RateProvider(rlog.Object);

        _service =
            new CurrencyService(
                _provider,
                slog.Object);
    }

    [Fact]
    public async Task Convert_EUR_To_USD_Returns_Expected_Value()
    {
        var expected =
            10m *
            (_provider.Get("EUR") /
             _provider.Get("USD"));

        var result =
            await _service.Convert(
                "EUR",
                "USD",
                10m);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task Convert_Same_Currency_Returns_Same_Amount()
    {
        var result =
            await _service.Convert(
                "EUR",
                "EUR",
                10m);

        result.Should().Be(10m);
    }

    [Fact]
    public async Task Convert_Should_Trim_Currency_Codes()
    {
        var expected =
            10m *
            (_provider.Get("EUR") /
             _provider.Get("USD"));

        var result =
            await _service.Convert(
                " eur ",
                " usd ",
                10m);

        result.Should().Be(expected);
    }

    //[Fact]
    //public async Task Convert_With_Negative_Amount_Should_Throw_DomainException()
    //{
    //    Func<Task> act =
    //        async () =>
    //            await _service.Convert(
    //                "EUR",
    //                "USD",
    //                -1m);

    //    await act.Should()
    //        .ThrowAsync<DomainException>();
    //}

    //[Fact]
    //public async Task Convert_With_Zero_Amount_Should_Throw_DomainException()
    //{
    //    Func<Task> act =
    //        async () =>
    //            await _service.Convert(
    //                "EUR",
    //                "USD",
    //                0m);

    //    await act.Should()
    //        .ThrowAsync<DomainException>();
    //}

    [Fact]
    public async Task Convert_With_Unknown_Base_Currency_Should_Throw_DomainException()
    {
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