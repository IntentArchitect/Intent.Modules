using Intent.Metadata.Models;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// Extension methods for <see cref="CSharpTemplateBase{TModel>"/>.
    /// </summary>
    public static class CSharpCodeExtensions
    {
        /// <summary>
        /// Returns a comma separated list of parameters for a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        public static string GetMethodParameters<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase(true)}"));
        }

        /// <summary>
        /// Returns a comma separated list of parameters for a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        public static string GetParameters<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase(true)}"));
        }

        /// <summary>
        /// Returns a comma separated list of arguments to be passed into a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        public static string GetArguments<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name}"));
        }
    }
}