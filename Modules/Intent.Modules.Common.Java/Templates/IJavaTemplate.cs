using Intent.Metadata.Models;
using Intent.Modules.Common.Java.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Templates;

public interface IJavaTemplate : IClassProvider, IDeclareImports
{
    void AddDependency(JavaDependency dependency);
    void AddImport(string fullyQualifiedType);
    string ImportType(string fullyQualifiedType);
}

public interface IJavaFileBuilderTemplate : IJavaTemplate
{
    JavaFile JavaFile { get; }
}