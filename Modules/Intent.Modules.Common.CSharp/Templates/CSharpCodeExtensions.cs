using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Templates
{
    public static class CSharpCodeExtensions
    {
        /// <summary>
        /// Returns a comma separated list of parameters for a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TParameterModel"></typeparam>
        /// <returns></returns>
        public static string GetMethodParameters<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase(true)}"));
        }

        /// <summary>
        /// Returns a comma separated list of parameters for a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TParameterModel"></typeparam>
        /// <returns></returns>
        public static string GetParameters<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{template.GetTypeName(x)} {x.Name.ToCamelCase(true)}"));
        }

        /// <summary>
        /// Returns a comma separated list of arguments to be passed into a method or constructor based on the <paramref name="parameters"/> argument.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TParameterModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetArguments<TModel, TParameterModel>(this CSharpTemplateBase<TModel> template, IEnumerable<TParameterModel> parameters)
            where TParameterModel : IHasName, IHasTypeReference
        {
            return string.Join(", ", parameters.Select(x => $"{x.Name}"));
        }
    }
}