using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Common.CSharp.Templates;

public interface ICSharpTemplate : IIntentTemplate, IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IHasFrameworkDependencies, ISupportsMigrations
{
    /// <summary>
    /// Used by the advanced mapping system to resolve references from models.
    /// </summary>
    ICSharpCodeContext RootCodeContext { get; }
    void AddUsing(string @namespace);
    string UseType(string fullName);
    void AddNugetDependency(INugetPackageInfo nugetPackageInfo);
    string GetFullyQualifiedTypeName(ITypeReference typeReference, string collectionFormat = null);
    string GetFullyQualifiedTypeName(string templateId, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null);

    void AddKnownType(string fullyQualifiedTypeName)    
    {
        CSharpTypesCache.AddKnownType(fullyQualifiedTypeName);
    }
}

public interface ICSharpFileBuilderTemplate : IFileBuilderTemplate, ICSharpTemplate
{
    CSharpFile CSharpFile { get; }
    IFileBuilderBase IFileBuilderTemplate.File => CSharpFile;
}