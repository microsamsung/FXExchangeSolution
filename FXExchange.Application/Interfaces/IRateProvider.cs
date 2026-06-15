using System.Collections.Immutable;

namespace FXExchange.Application.Interfaces;

public interface IRateProvider
{
    decimal Get(string currency);

    IReadOnlyDictionary<string, decimal>
        GetSnapshot();

    void UpdateSnapshot(
        ImmutableDictionary<string, decimal>
        rates);

    int Version { get; }

    DateTime LastUpdated { get; }
}