using System.Collections.Immutable;

namespace FXExchange.Application.Interfaces;

public interface IRateSource
{
    ValueTask<ImmutableDictionary<string, decimal>>
        GetRatesAsync(
            CancellationToken cancellationToken);
}