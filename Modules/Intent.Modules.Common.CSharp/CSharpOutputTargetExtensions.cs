using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Common.Templates
{
    public static class CSharpOutputTargetExtensions
    {
        public static string GetNamespace(this IOutputTarget target)
        {
            return string.Join(".", target.GetTargetPath().Select(x => x.Name.ToCSharpNamespace()));
        }
    }
}