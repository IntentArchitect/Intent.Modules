using System;
using System.Text.RegularExpressions;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.ModuleBuilder.Helpers
{
    public static class TemplateHelper
    {
        /// <summary>
        /// Use <see cref="IntentTemplateBase.TryGetExistingFileContent"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetExistingTemplateContent<T>(IntentFileTemplateBase<T> template)
        {
            return template.TryGetExistingFileContent(out var content)
                ? content
                : null;
        }

        private static readonly Regex _templateInheritsTagRegex = new Regex(
            @"(?<begin><#@[ ]*template[ ]+[\.a-zA-Z0-9=_\""#<> ]*inherits=\"")(?<type>[a-zA-Z0-9\._<>]+)(?<end>\""[ ]*#>)",
            RegexOptions.Compiled);

        public static string ReplaceTemplateInheritsTag(string templateContent, string inheritType)
        {
            return _templateInheritsTagRegex.Replace(templateContent, $"${{begin}}{inheritType}${{end}}");
        }
    }
}
