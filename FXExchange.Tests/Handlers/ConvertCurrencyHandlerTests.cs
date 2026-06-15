using FluentAssertions;
using FXExchange.Application.Commands;
using FXExchange.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FXExchange.Tests.Handlers;

public class ConvertCurrencyHandlerTests
{
    private readonly ConvertCurrencyHandler _handler;
    private readonly Mock<ICurrencyService> _service;

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
                    10))
            .ReturnsAsync(11.21985m);

        var command =
            new ConvertCurrencyCommand
            {
                BaseCurrency = "EUR",
                QuoteCurrency = "USD",
                Amount = 10
            };

        var result =
            await _handler.Handle(
                command,
                CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Value.Should()
            .BeApproximately(
                11.21985m,
                0.0001m);
    }
}