using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Serilog.Core;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.Logging.Serilog.BoundedLoggingDestructuringPolicyTemplate", Version = "1.0")]

namespace ArchTemplateResearch.Api.Logging;

/// <summary>
/// A Serilog destructuring policy that prevents excessive logging by:
/// 1. Limiting collection sizes
/// 2. Truncating objects with too many properties
/// 3. Replacing problematic property types (like streams and byte arrays) with placeholders
/// 4. Handling exceptions during property access to prevent logging failures
/// 
/// This policy helps maintain logging performance by controlling the size of log events
/// and preventing runaway memory consumption from large object graphs.
/// </summary>
public class BoundedLoggingDestructuringPolicy : IDestructuringPolicy
{
    private static readonly ConcurrentDictionary<Type, PropertyAnalysisResult> PropertyCache = new();

    public bool TryDestructure(object? value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        // No need for us to look at
        if (value is null or string)
        {
            result = null;
            return false;
        }

        // Check for scalar value
        if (ShouldOmitType(value.GetType(), out var replacementText))
        {
            result = new ScalarValue(replacementText);
            return true;
        }

        // More complex checks...

        if (value is IDictionary dict && FilterDictionaryIfNeeded(dict, propertyValueFactory, out result))
        {
            return true;
        }

        if (value is IEnumerable enumerable && FilterEnumerableIfNeeded(enumerable, propertyValueFactory, out result))
        {
            return true;
        }

        var valueType = value.GetType();
        if (valueType.IsClass && FilterObjectIfNeeded(value, valueType, propertyValueFactory, out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    private static bool FilterDictionaryIfNeeded(IDictionary dictionary, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        const int maxEntryCount = 30;
        if (dictionary.Count > maxEntryCount)
        {
            result = new DictionaryValue(dictionary
                .OfType<DictionaryEntry>()
                .Take(maxEntryCount)
                .Append(new DictionaryEntry("", $"... (and {dictionary.Count - maxEntryCount} more entries)"))
                .Select(s => new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(s.Key), propertyValueFactory.CreatePropertyValue(s.Value, true))));
            return true;
        }

        result = null;
        return false;
    }

    private static bool FilterEnumerableIfNeeded(IEnumerable enumerable, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        const int maxSequenceCount = 10;
        if (enumerable is ICollection collection)
        {
            if (collection.Count <= maxSequenceCount)
            {
                result = null;
                return false;
            }
            var truncatedList = new List<object?>(enumerable.Cast<object?>().Take(maxSequenceCount));
            truncatedList.Add($"... (and {collection.Count - maxSequenceCount} more items)");
            result = new SequenceValue(truncatedList.Select(s => propertyValueFactory.CreatePropertyValue(s, true)));
            return true;
        }

        var count = 0;
        var truncatedEnumeration = new List<object?>();
        var enumerator = enumerable.GetEnumerator();
        using var _ = enumerator as IDisposable;

        while (enumerator.MoveNext() && count < maxSequenceCount)
        {
            count++;
            truncatedEnumeration.Add(enumerator.Current);
        }

        if (count >= maxSequenceCount)
        {
            truncatedEnumeration.Add($"... (and more items)");
        }

        result = new SequenceValue(truncatedEnumeration.Select(s => propertyValueFactory.CreatePropertyValue(s, true)));
        return true;
    }

    private static bool FilterObjectIfNeeded(object obj, Type objectType, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        const int maxPropertyCount = 30;
        var analysis = PropertyCache.GetOrAdd(objectType, AnalyzeType);
        if (!analysis.HasPropertiesToOmit)
        {
            result = null;
            return false;
        }

        var filteredProperties = new List<KeyValuePair<string, object?>>();
        foreach (var propInfo in analysis.Properties)
        {
            if (propInfo.ShouldOmit)
            {
                filteredProperties.Add(new KeyValuePair<string, object?>(propInfo.Name, propInfo.ReplacementText));
            }
            else
            {
                try
                {
                    filteredProperties.Add(new KeyValuePair<string, object?>(propInfo.Name, propInfo.PropertyInfo.GetValue(obj)));
                }
                catch (Exception ex)
                {
                    filteredProperties.Add(new KeyValuePair<string, object?>(propInfo.Name, $"[Error accessing property: {ex.Message}]"));
                }
            }

            if (filteredProperties.Count < maxPropertyCount)
            {
                continue;
            }

            if (analysis.Properties.Count > maxPropertyCount)
            {
                filteredProperties.Add(new KeyValuePair<string, object?>("...", $"... (and {analysis.Properties.Count - maxPropertyCount} more properties)"));
            }
            break;
        }

        result = new StructureValue(filteredProperties.Select(s => new LogEventProperty(s.Key, propertyValueFactory.CreatePropertyValue(s.Value, true))));
        return true;
    }

    private static bool ShouldOmitType(Type type, [NotNullWhen(true)] out string? replacementText)
    {
        if (type == typeof(byte[]))
        {
            replacementText = "[byte[] omitted]";
            return true;
        }

        if (type.IsAssignableTo(typeof(Stream)))
        {
            replacementText = "[Stream omitted]";
            return true;
        }

        replacementText = null;
        return false;
    }

    private static PropertyAnalysisResult AnalyzeType(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyInfos = new List<PropertyAnalysisInfo>();
        var hasPropertiesToOmit = false;

        foreach (var prop in properties)
        {
            var shouldOmit = ShouldOmitType(prop.PropertyType, out var omitText);
            propertyInfos.Add(new PropertyAnalysisInfo(
                prop,
                prop.Name,
                shouldOmit,
                omitText ?? string.Empty
            ));

            if (shouldOmit)
            {
                hasPropertiesToOmit = true;
            }
        }

        return new PropertyAnalysisResult(propertyInfos, hasPropertiesToOmit);
    }

    private sealed record PropertyAnalysisResult(IReadOnlyList<PropertyAnalysisInfo> Properties, bool HasPropertiesToOmit);

    private sealed record PropertyAnalysisInfo(PropertyInfo PropertyInfo, string Name, bool ShouldOmit, string ReplacementText);
}