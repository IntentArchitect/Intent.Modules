using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

public interface ICSharpTemplate : IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IRoslynMerge, IDeclareUsings, IHasFrameworkDependencies
{
    string UseType(string fullName);
    string GetTypeName(ITypeReference type);
    string GetTypeName(string templateIdOrRole, TemplateDiscoveryOptions options = null);
    string GetTypeName(string templateIdOrRole, string modelId, TemplateDiscoveryOptions options = null);
    string GetTypeName(string templateIdOrRole, IMetadataModel model, TemplateDiscoveryOptions options = null);
    ISoftwareFactoryExecutionContext ExecutionContext { get; }
    IFileMetadata FileMetadata { get; }
}