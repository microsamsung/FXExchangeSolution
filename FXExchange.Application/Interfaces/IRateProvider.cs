using System.Collections.Generic;

namespace FXExchange.Application.Interfaces;

public interface IRateProvider
{
    decimal Get(string currency);

    IReadOnlyDictionary<string, decimal> GetSnapshot();
}