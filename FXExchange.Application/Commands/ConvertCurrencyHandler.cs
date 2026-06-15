using MediatR;
using Microsoft.Extensions.Logging;
using FXExchange.Application.Common;
using FXExchange.Application.Interfaces;
using FXExchange.Domain.Exceptions;

namespace FXExchange.Application.Commands;

public sealed class ConvertCurrencyHandler :
    IRequestHandler<
        ConvertCurrencyCommand,
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
            var result =
                await _service.Convert(
                    request.BaseCurrency,
                    request.QuoteCurrency,
                    request.Amount);

            return Result<decimal>.Ok(result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                ex,
                "Currency conversion validation failed");

            return Result<decimal>.Fail(
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Currency conversion failed");

            return Result<decimal>.Fail(
                "Unexpected conversion error");
        }
    }
}