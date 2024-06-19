#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpParameter : ICSharpReferenceable, ICSharpMetadataBase
{
    string Type { get; }
    string Name { get; }
    string DefaultValue { get; }
    string XmlDocComment { get; }
}