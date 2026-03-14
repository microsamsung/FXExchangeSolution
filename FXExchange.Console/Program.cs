using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FXExchange.Application.Commands;
using FXExchange.Application.Interfaces;
using FXExchange.Infrastructure.Services;
using FXExchange.Infrastructure.Providers;
using FXExchange.Persistence.Context;
using FXExchange.Persistence.Repository;
using FXExchange.Persistence.UnitOfWork;
using FXExchange.Domain.Entities;
using System.Globalization;
using FXExchange.Infrastructure.BackgroundServices;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    // Logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
    });

    // Database (InMemory for assignment requirement)
    services.AddDbContext<FxDbContext>(options =>
        options.UseInMemoryDatabase("FxDb"));

    // MediatR (works for all versions)
    services.AddMediatR(
        typeof(ConvertCurrencyHandler).Assembly);

    // Infrastructure
    services.AddSingleton<RateProvider>();

    services.AddHostedService<RateRefreshService>();

    services.AddScoped<ICurrencyService,
        CurrencyService>();

    // Repository
    services.AddScoped<IRepository<ExchangeRate>,
        ExchangeRepository>();

    services.AddScoped<IUnitOfWork,
        UnitOfWork>();



})
.Build();

var logger =
host.Services.GetRequiredService<ILoggerFactory>()
.CreateLogger("Program");

var mediator =
host.Services.GetRequiredService<IMediator>();

//----------------------------------------------------------------------------------------------------

Console.WriteLine("==================================");
Console.WriteLine("        FX Exchange System        ");
Console.WriteLine("==================================");

while (true)
{
    try
    {
        Console.WriteLine();
        Console.WriteLine("Enter currency pair (example EUR/USD):");

        var pair = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(pair))
        {
            Console.WriteLine("Error: Currency pair required");
            continue;
        }

        Console.WriteLine("Enter amount:");

        var amountInput = Console.ReadLine();

        if (!decimal.TryParse(
            amountInput,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out decimal amount))
        {
            Console.WriteLine("Error: Invalid amount");
            continue;
        }

        var currencies = pair.Split('/',
            StringSplitOptions.RemoveEmptyEntries);

        if (currencies.Length != 2)
        {
            Console.WriteLine("Error: Invalid currency pair");
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

        logger.LogInformation("Conversion requested {Base} {Quote} {Amount}", baseCurrency,quoteCurrency,
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
            Console.WriteLine("Exchanged amount:");
            Console.WriteLine(Math.Round(result.Value, 4));
            Console.WriteLine("-------------------------------");

            logger.LogInformation("Conversion completed {Result}",
            result.Value);
        }
        else
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Error: {result.Error}");
            Console.WriteLine("-------------------------------");

            logger.LogWarning(
            "Conversion failed {Error}",
            result.Error);
        }

    }
    catch (Exception ex)
    {
        logger.LogError(ex,"Unhandled exception");

        Console.WriteLine("Unexpected error occurred");
    }

    Console.WriteLine();
    Console.WriteLine("Press Q to quit or any key to continue");

    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.Q)
        break;

    Console.Clear();

    Console.WriteLine("==================================");
    Console.WriteLine("        FX Exchange System        ");
    Console.WriteLine("==================================");
}

await host.StopAsync();
host.Dispose();