using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public interface IHasClassDetails : ITemplate
    {
        string Namespace { get; }
        string ClassName { get; }
    }

    public static class IHasClassDetailsExtensions
    {
        public static string FullTypeName(this IHasClassDetails item)
        {
            if (string.IsNullOrWhiteSpace(item.Namespace))
            {
                return item.ClassName;
            }
            return item.Namespace + "." + item.ClassName;
        }
    }
}
