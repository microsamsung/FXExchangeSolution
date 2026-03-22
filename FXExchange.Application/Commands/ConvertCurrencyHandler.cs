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
            if (request is null)
                throw new DomainException(
                    "Request cannot be null",
                    "FX_REQUEST_NULL");

            cancellationToken
                .ThrowIfCancellationRequested();

            _logger.LogInformation(
            "Conversion request {Base}->{Quote} Amount {Amount}",
            request.BaseCurrency,
            request.QuoteCurrency,
            request.Amount);

            var result =
                await _service.Convert(
                    request.BaseCurrency,
                    request.QuoteCurrency,
                    request.Amount);

            _logger.LogInformation(
            "Conversion successful Result {Result}",
            result);

            return Result<decimal>.Ok(result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
            ex,
            "Business validation failed {Base} {Quote}",
            request?.BaseCurrency,
            request?.QuoteCurrency);

            return Result<decimal>.Fail(
                ex.Message);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
            "Conversion cancelled");

            return Result<decimal>.Fail(
                "Operation cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Unexpected failure {Base} {Quote}",
            request?.BaseCurrency,
            request?.QuoteCurrency);

            return Result<decimal>.Fail(
                "Unexpected conversion error");
        }
    }
}