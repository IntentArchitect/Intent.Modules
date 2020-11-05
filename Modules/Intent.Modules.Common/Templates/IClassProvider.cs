using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public interface IClassProvider : ITemplate
    {
        string Namespace { get; }
        string ClassName { get; }
    }

    public static class IClassProviderExtensions
    {
        public static string FullTypeName(this IClassProvider item)
        {
            if (string.IsNullOrWhiteSpace(item.Namespace))
            {
                return item.ClassName;
            }
            return item.Namespace + "." + item.ClassName;
        }
    }
}
