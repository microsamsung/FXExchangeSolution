using MediatR;
using Microsoft.Extensions.Logging;
using FXExchange.Application.Common;
using FXExchange.Application.Interfaces;

namespace FXExchange.Application.Commands;

public sealed class ConvertCurrencyHandler :
    IRequestHandler<ConvertCurrencyCommand,
    Result<decimal>>
{
    private readonly ICurrencyService _service;

    private readonly ILogger<ConvertCurrencyHandler>
        _logger;

    public ConvertCurrencyHandler(
        ICurrencyService service,
        ILogger<ConvertCurrencyHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(service);

        ArgumentNullException.ThrowIfNull(logger);

        _service = service;

        _logger = logger;
    }

    public async Task<Result<decimal>> Handle(
        ConvertCurrencyCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(
            "Processing conversion {Base} to {Quote} amount {Amount}",
            request.BaseCurrency,
            request.QuoteCurrency,
            request.Amount);

            var result =
                await _service.Convert(
                    request.BaseCurrency,
                    request.QuoteCurrency,
                    request.Amount);

            _logger.LogInformation(
            "Conversion completed result {Result}",
            result);

            return Result<decimal>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Conversion failed");

            return Result<decimal>
                .Fail(ex.Message);
        }
    }
}