using System.Collections.Generic;

namespace Appy.Configuration.Common;

public static class IDictionaryExtensions
{
    public static IDictionary<TKey, TValue> AddValue<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key, TValue value)
        where TKey : notnull
    {
        dict.Add(key, value);
        return dict;
    }
}