using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

public interface ICSharpTemplate : IIntentTemplate, IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IHasFrameworkDependencies
{
    string UseType(string fullName);
    string GetFullyQualifiedTypeName(ITypeReference typeReference, string collectionFormat = null);
    string GetFullyQualifiedTypeName(string templateId, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null);
    string GetFullyQualifiedTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null);
}

public interface ICSharpFileBuilderTemplate : ICSharpTemplate
{
    CSharpFile CSharpFile { get; }
}