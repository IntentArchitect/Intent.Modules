using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// Provides helper methods for working with dictionaries containing string keys and object values.
    /// </summary>
    internal class DictionaryHelper
    {
        /// <summary>
        /// Merges the contents of <paramref name="mergeDict"/> into <paramref name="baseDict"/>.
        /// If both dictionaries contain a value for the same key and those values are themselves dictionaries,
        /// the method recursively merges them. Otherwise, the value from <paramref name="mergeDict"/> overwrites
        /// the value in <paramref name="baseDict"/>.
        /// </summary>
        /// <param name="baseDict">The base dictionary to merge into.</param>
        /// <param name="mergeDict">The dictionary whose entries will be merged into the base dictionary.</param>
        /// <returns>The merged dictionary, which is the modified <paramref name="baseDict"/>.</returns>
        public static Dictionary<string, object> MergeDictionaries(
            Dictionary<string, object> baseDict,
            Dictionary<string, object> mergeDict)
        {
            foreach (var kvp in mergeDict)
            {
                if (baseDict.TryGetValue(kvp.Key, out var existingValue) &&
                    existingValue is Dictionary<string, object> existingDict &&
                    kvp.Value is Dictionary<string, object> newDict)
                {
                    MergeDictionaries(existingDict, newDict);
                }
                else
                {
                    baseDict[kvp.Key] = kvp.Value;
                }
            }

            return baseDict;
        }

    }
}
