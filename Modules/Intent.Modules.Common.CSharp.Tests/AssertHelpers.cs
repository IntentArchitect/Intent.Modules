using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Tests
{
    public static class AssertHelpers
    {
        public static Action<T>[] ToElementInspectors<T>(
            this IReadOnlyCollection<T> expectedCollection,
            IReadOnlyCollection<T> actualCollection, Action<(T actualItem, T expectedItem)> assert)
        {
            return expectedCollection
                .Zip(actualCollection, (expectedItem, actualItem) => (expectedItem, actualItem))
                .Select(tuple => new Action<T>(actualItem =>
                {
                    if (!tuple.actualItem.Equals(actualItem))
                    {
                        throw new Exception();
                    }

                    assert((tuple.actualItem, tuple.expectedItem));
                }))
                .ToArray();
        }
    }
}