using MediatR;
using FXExchange.Application.Common;

namespace FXExchange.Application.Commands;

public sealed class ConvertCurrencyCommand : IRequest<Result<decimal>>
{
    public string BaseCurrency { get; set; }

    public string QuoteCurrency { get; set; }

    public decimal Amount { get; set; }
}