using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    public static class JavaCodeExtensions
    {
        public static string GetMethodParameters<TModel, TParameterModel>(this JavaTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase()}"));
        }

        public static string GetParameters<TModel, TParameterModel>(this JavaTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel: IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase()}"));
        }

        public static string GetArguments<TModel, TParameterModel>(this JavaTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name}"));
        }
    }
}