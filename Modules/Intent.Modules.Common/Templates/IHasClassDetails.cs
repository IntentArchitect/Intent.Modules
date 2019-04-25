namespace Intent.Modules.Common.Templates
{
    public interface IHasClassDetails
    {
        string Namespace { get; }
        string ClassName { get; }
    }

    public static class IHasClassDetailsExtensions
    {
        public static string FullTypeName(this IHasClassDetails item)
        {
            return item.Namespace + "." + item.ClassName;
        }
    }
}
