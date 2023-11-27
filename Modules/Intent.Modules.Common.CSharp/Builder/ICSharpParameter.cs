namespace Intent.Modules.Common.CSharp.Builder
{
    public interface ICSharpParameter
    {
        string Type { get; }
        string Name { get; }
        string DefaultValue { get; }
        string XmlDocComment { get; }    }
}
