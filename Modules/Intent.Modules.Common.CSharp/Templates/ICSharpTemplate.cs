﻿using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using System;
using Intent.Modules.Common.CSharp.Nuget;

namespace Intent.Modules.Common.CSharp.Templates;

public interface ICSharpTemplate : IIntentTemplate, IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IHasFrameworkDependencies, ISupportsMigrations, IHasGlobalUsings
{
    /// <summary>
    /// Used by the advanced mapping system to resolve references from models.
    /// </summary>
    ICSharpCodeContext RootCodeContext { get; }
    void AddUsing(string @namespace);
    void RemoveUsing(string @namespace) => throw new NotSupportedException();
    string UseType(string fullName);
    void AddNugetDependency(INugetPackageInfo nugetPackageInfo);
    void AddNugetDependency(INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options);
    void RemoveNugetDependency(string packageName) => throw new NotSupportedException();
    string GetFullyQualifiedTypeName(ITypeReference typeReference, string collectionFormat = null);
    string GetFullyQualifiedTypeName(string templateId, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null);

    void AddKnownType(string fullyQualifiedTypeName)    
    {
        CSharpTypesCache.AddKnownType(fullyQualifiedTypeName);
    }

    IOutputTarget Project { get; }
}

public interface ICSharpFileBuilderTemplate : IFileBuilderTemplate, ICSharpTemplate
{
    CSharpFile CSharpFile { get; }
    IFileBuilderBase IFileBuilderTemplate.File => CSharpFile;
}