using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common
{
    public static class ElementExtensions
    {
        public static IEnumerable<IElement> GetElementsOfType(this IEnumerable<IElement> elements, string typeId, bool recursiveSearch = false)
        {
            return GetMatchedElements(elements, e => e.SpecializationTypeId == typeId, recursiveSearch);
        }

        public static IEnumerable<IElement> GetMatchedElements(this IEnumerable<IElement> elements, Func<IElement, bool> matchFunc, bool recursiveSearch)
        {
            var results = new List<IElement>();
            foreach (var element in elements)
            {
                if (matchFunc(element))
                {
                    results.Add(element);
                }

                if (recursiveSearch)
                {
                    results.AddRange(GetMatchedElements(element.ChildElements, matchFunc, recursiveSearch));
                }
            }

            return results;
        }
    }
}