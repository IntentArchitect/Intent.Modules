using System;
using System.Collections.Generic;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.ModuleBuilder.Api
{
    public static class ApiModelExtensions
    {
        public static string GetApiModelName(this ICanBeReferencedType element)
        {
            return $"{element.Name.ToCSharpIdentifier()}Model";
        }
    }
}
