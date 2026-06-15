using FluentAssertions;
using FXExchange.Application.Commands;
using FXExchange.Application.Validators;

namespace FXExchange.Tests.Validation;

public class ConvertCurrencyValidatorTests
{
    private readonly ConvertCurrencyValidator _validator;

    public ConvertCurrencyValidatorTests()
    {
        _validator = new ConvertCurrencyValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsSuccess()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "USD",
            Amount = 10m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyBaseCurrency_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "",
            QuoteCurrency = "USD",
            Amount = 10m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.BaseCurrency));
    }

    [Fact]
    public void Validate_WithNullBaseCurrency_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = null!,
            QuoteCurrency = "USD",
            Amount = 10m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.BaseCurrency));
    }

    [Fact]
    public void Validate_WithEmptyQuoteCurrency_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "",
            Amount = 10m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.QuoteCurrency));
    }

    [Fact]
    public void Validate_WithNullQuoteCurrency_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = null!,
            Amount = 10m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.QuoteCurrency));
    }

    [Fact]
    public void Validate_WithZeroAmount_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "USD",
            Amount = 0m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.Amount));
    }

    [Fact]
    public void Validate_WithNegativeAmount_ReturnsValidationError()
    {
        var command = new ConvertCurrencyCommand
        {
            BaseCurrency = "EUR",
            QuoteCurrency = "USD",
            Amount = -100m
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == nameof(command.Amount));
    }

}