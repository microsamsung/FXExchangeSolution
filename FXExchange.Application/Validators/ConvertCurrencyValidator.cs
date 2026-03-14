using FluentValidation;
using FXExchange.Application.Commands;

namespace FXExchange.Application.Validators
{
    public class ConvertCurrencyValidator : AbstractValidator<ConvertCurrencyCommand>
    {
        public ConvertCurrencyValidator()
        {
            RuleFor(x => x.BaseCurrency)
                .NotEmpty();

            RuleFor(x => x.QuoteCurrency)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
