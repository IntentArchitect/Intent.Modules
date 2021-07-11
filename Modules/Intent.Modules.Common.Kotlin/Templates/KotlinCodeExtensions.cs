using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Kotlin.Templates
{
    public static class KotlinCodeExtensions
    {
        public static string GetMethodParameters<TModel, TParameterModel>(this KotlinTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name.ToCamelCase()}: {template.GetTypeName(x)}"));
        }

        public static string GetParameters<TModel, TParameterModel>(this KotlinTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel: IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name.ToCamelCase()}: {template.GetTypeName(x)}"));
        }

        public static string GetArguments<TModel, TParameterModel>(this KotlinTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name}"));
        }
    }
}