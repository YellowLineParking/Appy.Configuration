using System.Collections.Generic;

namespace Appy.Configuration.Common;

public static class CollectionExtensions
{
    public static TCollection AddItem<TCollection, TElement>(
        this TCollection collection,
        TElement item)
        where TCollection : ICollection<TElement>
    {
        collection.Add(item);
        return collection;
    }
}