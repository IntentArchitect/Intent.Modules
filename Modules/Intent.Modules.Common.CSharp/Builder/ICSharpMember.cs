#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpMember<out TSpecialization> : ICSharpDeclaration<TSpecialization>, ICodeBlock
    where TSpecialization : ICSharpDeclaration<TSpecialization>;