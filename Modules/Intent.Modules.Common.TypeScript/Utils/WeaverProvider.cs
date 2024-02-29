using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Utils;

public static class WeaverProvider
{
    public delegate string InsertImportDirectivesDelegate(string existingContent, IReadOnlyCollection<TypeScriptImport> imports);

    private static InsertImportDirectivesDelegate _insertImportDirectivesProvider;
    public static void SetInsertImportDirectivesProvider(InsertImportDirectivesDelegate provider)
    {
        _insertImportDirectivesProvider = provider;
    }

    public static string InsertImportDirectives(string existingContent, IReadOnlyCollection<TypeScriptImport> imports)
    {
        if (_insertImportDirectivesProvider is null)
        {
            throw new InvalidOperationException($"Cannot invoke {nameof(InsertImportDirectives)} without registering a delegate provider for it");
        }
        return _insertImportDirectivesProvider(existingContent, imports);
    }
}

public class TypeScriptImport
{
    public TypeScriptImport(string type, string location)
    {
        Type = type;
        Location = location;
    }
        
    public string Type { get; }
    public string Location { get; }
}