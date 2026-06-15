using FluentAssertions;
using FXExchange.Application.Commands;
using FXExchange.Application.Interfaces;
using FXExchange.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Handlers;

public class ConvertCurrencyHandlerTests
{
    private readonly ConvertCurrencyHandler _handler;

    private readonly Mock<ICurrencyService>
        _service;

    public ConvertCurrencyHandlerTests()
    {
        _service =
            new Mock<ICurrencyService>();

        var logger =
            new Mock<ILogger<ConvertCurrencyHandler>>();

        _handler =
            new ConvertCurrencyHandler(
                _service.Object,
                logger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success()
    {
        _service
            .Setup(x =>
                x.Convert(
                    "EUR",
                    "USD",
                    10m))
            .ReturnsAsync(11.21985m);

        var command =
            new ConvertCurrencyCommand
            {
                BaseCurrency = "EUR",
                QuoteCurrency = "USD",
                Amount = 10m
            };

        var result =
            await _handler.Handle(
                command,
                CancellationToken.None);

        result.Success.Should().BeTrue();

        result.Value.Should()
            .Be(11.21985m);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_DomainException_Is_Thrown()
    {
        _service
            .Setup(x =>
                x.Convert(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>()))
            .ThrowsAsync(
                new DomainException(
                    "Unknown currency",
                    "FX_UNKNOWN_CURRENCY"));

        var command =
            new ConvertCurrencyCommand
            {
                BaseCurrency = "AAA",
                QuoteCurrency = "USD",
                Amount = 10m
            };

        var result =
            await _handler.Handle(
                command,
                CancellationToken.None);

        result.Success.Should().BeFalse();

        result.Error.Should()
            .Be("Unknown currency");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Unexpected_Exception_Is_Thrown()
    {
        _service
            .Setup(x =>
                x.Convert(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Unexpected error"));

        var command =
            new ConvertCurrencyCommand
            {
                BaseCurrency = "EUR",
                QuoteCurrency = "USD",
                Amount = 10m
            };

        var result =
            await _handler.Handle(
                command,
                CancellationToken.None);

        result.Success.Should().BeFalse();

        result.Error.Should()
            .Be("Unexpected conversion error");
    }

}