using FXExchange.Application.Interfaces;
using FXExchange.Persistence.Context;
using Microsoft.Extensions.Logging;

namespace FXExchange.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly FxDbContext _context;

    private readonly ILogger<UnitOfWork>
        _logger;

    public UnitOfWork(
        FxDbContext context,
        ILogger<UnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(logger);

        _context = context;

        _logger = logger;
    }

    public async Task Save( )
    {
        try
        {
            _logger.LogInformation(
            "Saving database changes");

            await _context
                .SaveChangesAsync();

            _logger.LogInformation(
            "Database saved");
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Database save failed");

            throw;
        }
    }
}