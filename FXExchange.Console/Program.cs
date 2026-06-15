using FXExchange.Application.Commands;
using FXExchange.Application.Interfaces;
using FXExchange.Infrastructure.Providers;
using FXExchange.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

        services.AddMediatR(
            typeof(ConvertCurrencyHandler).Assembly);

        services.AddScoped<ICurrencyService, CurrencyService>();

        services.AddSingleton<IRateProvider, RateProvider>();

        services.AddSingleton<IRateSource,HardcodedRateSource>();

        services.AddHostedService<RateRefreshService>();


    })
    .Build();

var logger =
    host.Services
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Program");

var mediator =
    host.Services
        .GetRequiredService<IMediator>();

Console.WriteLine("==================================");
Console.WriteLine("        FX Exchange System        ");
Console.WriteLine("==================================");

while (true)
{
    try
    {
        Console.WriteLine();
        Console.WriteLine(
            "Enter currency pair (example EUR/USD):");

        var pair = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(pair))
        {
            Console.WriteLine(
                "Error: Currency pair is required");
            continue;
        }

        var currencies =
            pair.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

        if (currencies.Length != 2)
        {
            Console.WriteLine(
                "Error: Currency pair must be in format EUR/USD");
            continue;
        }

        var baseCurrency =
            currencies[0]
                .Trim()
                .ToUpperInvariant();

        var quoteCurrency =
            currencies[1]
                .Trim()
                .ToUpperInvariant();

        if (baseCurrency.Length != 3 ||
            quoteCurrency.Length != 3)
        {
            Console.WriteLine(
                "Error: Currency codes must contain exactly 3 letters");
            continue;
        }

        decimal amount;

        while (true)
        {
            Console.WriteLine("Enter amount:");

            var amountInput =
                Console.ReadLine();

            if (!decimal.TryParse(
                    amountInput,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out amount))
            {
                Console.WriteLine(
                    "Error: Invalid amount");

                continue;
            }

            if (amount <= 0)
            {
                Console.WriteLine(
                    "Error: Amount must be greater than zero");

                continue;
            }

            break;
        }

        logger.LogInformation(
            "Conversion requested {Base}->{Quote} Amount {Amount}",
            baseCurrency,
            quoteCurrency,
            amount);

        var result =
            await mediator.Send(
                new ConvertCurrencyCommand
                {
                    BaseCurrency = baseCurrency,
                    QuoteCurrency = quoteCurrency,
                    Amount = amount
                });

        Console.WriteLine();

        if (result.Success)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine(
                $"Converted Amount: {result.Value:F6}");
            Console.WriteLine("-------------------------------");

            logger.LogInformation(
                "Conversion completed. Result: {Result}",
                result.Value);
        }
        else
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine(
                $"Error: {result.Error}");
            Console.WriteLine("-------------------------------");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Unhandled exception");

        Console.WriteLine(
            "Unexpected error occurred.");
    }

    Console.WriteLine();
    Console.WriteLine(
        "Press Q to quit or any other key to continue");

    var key =
        Console.ReadKey();

    if (key.Key == ConsoleKey.Q)
        break;

    Console.Clear();

    Console.WriteLine("==================================");
    Console.WriteLine("        FX Exchange System        ");
    Console.WriteLine("==================================");
}

await host.StopAsync();
host.Dispose();