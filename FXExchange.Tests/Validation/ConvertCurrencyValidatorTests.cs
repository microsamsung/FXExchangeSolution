using FluentAssertions;
using FXExchange.Application.Commands;
using FXExchange.Application.Validators;

namespace FXExchange.Tests.Validation;

public class ConvertCurrencyValidatorTests
{

    [Fact]
    public void ShouldValidateValidRequest()
    {
        var validator = new ConvertCurrencyValidator();

        var command = new ConvertCurrencyCommand
            {
                BaseCurrency = "EUR",

                QuoteCurrency = "USD",

                Amount = 10
            };

        var result = validator.Validate(command);

        result.IsValid
              .Should()
              .BeTrue();
    }

    [Fact]
    public void ShouldFailEmptyCurrency()
    {
        var validator = new ConvertCurrencyValidator();

        var command = new ConvertCurrencyCommand
            {
                BaseCurrency = "",

                QuoteCurrency = "USD",

                Amount = 10
            };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ShouldFailZeroAmount()
    {
        var validator = new ConvertCurrencyValidator();

        var command = new ConvertCurrencyCommand
            {
                BaseCurrency = "EUR",

                QuoteCurrency = "USD",

                Amount = 0
            };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

}