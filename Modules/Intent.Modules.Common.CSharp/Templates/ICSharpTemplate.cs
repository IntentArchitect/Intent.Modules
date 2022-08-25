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
}

public interface ICSharpFileBuilderTemplate : ICSharpTemplate
{
    CSharpFile CSharpFile { get; }
}