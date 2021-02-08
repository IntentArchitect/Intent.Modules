using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// Template that provides a OOP class type - Class name and namespace (package).
    /// </summary>
    public interface IClassProvider : ITemplate
    {
        string Namespace { get; }
        string ClassName { get; }
    }

    public static class IClassProviderExtensions
    {
        /// <summary>
        /// Returns the fully qualified class name.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
