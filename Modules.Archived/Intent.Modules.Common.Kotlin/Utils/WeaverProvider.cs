using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.Kotlin.Utils;

public static class WeaverProvider
{
    public delegate string InsertImportDirectivesDelegate(string existingContent, string packageName, IReadOnlyCollection<string> imports);

    private static InsertImportDirectivesDelegate _insertImportDirectivesProvider;
    public static void SetInsertImportDirectivesProvider(InsertImportDirectivesDelegate provider)
    {
        _insertImportDirectivesProvider = provider;
    }

    public static string InsertImportDirectives(string existingContent, string packageName, IReadOnlyCollection<string> imports)
    {
        if (_insertImportDirectivesProvider is null)
        {
            throw new InvalidOperationException($"Cannot invoke {nameof(InsertImportDirectives)} without registering a delegate provider for it");
        }
        return _insertImportDirectivesProvider(existingContent, packageName, imports);
    }
}