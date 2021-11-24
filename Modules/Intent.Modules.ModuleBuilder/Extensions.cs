using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.ModuleBuilder
{
    public static class Extensions
    {
        public static string GetFolderPathWithout<TModel>(
            this IntentTemplateBase<TModel> template,
            string partToExclude,
            params string[] additionalFolders)
            where TModel : IHasFolder
        {
            var split = template.GetFolderPath(additionalFolders).Split('/');

            return string.Join('/', split.Where(part => part != partToExclude));
        }

        public static string GetNamespaceWithSingleOnlyOf<TModel>(
            this CSharpTemplateBase<TModel> template,
            string singleOnlyOf,
            params string[] additionalFolders)
            where TModel : IHasFolder
        {
            var @string = template.GetNamespace(additionalFolders);
            const char separator = '.';

            var split = @string.Split(separator);
            if (split.Count(part => part == singleOnlyOf) <= 1)
            {
                return @string;
            }

            var hasSeenAlready = false;
            return string.Join(separator, split
                .Select(part =>
                {
                    if (part != singleOnlyOf)
                    {
                        return part;
                    }

                    if (hasSeenAlready)
                    {
                        return null;
                    }

                    hasSeenAlready = true;
                    return part;

                })
                .Where(x => x != null));
        }
    }
}
