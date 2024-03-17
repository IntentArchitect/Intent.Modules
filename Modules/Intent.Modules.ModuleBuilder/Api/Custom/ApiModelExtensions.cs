using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.ModuleBuilder.Api
{
    public static class ApiModelExtensions
    {
        public static string GetApiModelName(this ICanBeReferencedType element)
        {
            return $"{element.Name.ToCSharpIdentifier()}Model";
        }

        public static string FormatForCollection(this string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        public static string GetCreationOptionName(this ElementCreationOptionModel option)
        {
            if (option.GetOptionSettings().ApiModelName() != null)
            {
                return option.GetOptionSettings().ApiModelName();
            }
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.Pluralize() : name;
        }
    }
}
