using System.Collections.Immutable;

namespace FXExchange.Application.Interfaces;

public interface IRateSource
{
    Task<ImmutableDictionary<string, decimal>> GetRatesAsync(CancellationToken cancellationToken);
}