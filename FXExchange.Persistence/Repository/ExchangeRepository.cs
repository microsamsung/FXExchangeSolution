using Microsoft.EntityFrameworkCore;
using FXExchange.Application.Interfaces;
using FXExchange.Domain.Entities;
using FXExchange.Persistence.Context;
using Microsoft.Extensions.Logging;

namespace FXExchange.Persistence.Repository;

public sealed class ExchangeRepository :
    IRepository<ExchangeRate>
{
    private readonly FxDbContext _context;

    private readonly ILogger<ExchangeRepository>
        _logger;

    public ExchangeRepository(
        FxDbContext context,
        ILogger<ExchangeRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(logger);

        _context = context;

        _logger = logger;
    }

    public async Task<ExchangeRate?> Get(
        string currency)
    {
        try
        {
            return await _context.Rates
                .AsNoTracking()
                .FirstOrDefaultAsync(
                x => x.Currency == currency);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Get failed");

            throw;
        }
    }

    public async Task Add( ExchangeRate entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Rates.AddAsync(entity);
    }

    public Task Update( ExchangeRate entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Rates.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete( ExchangeRate entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Rates.Remove(entity);

        return Task.CompletedTask;
    }
}