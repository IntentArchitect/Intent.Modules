using Intent.Modules.Common.Templates;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Intent.Modules.ModuleBuilder.Helpers
{
    public static class TemplateHelper
    {
        public static string GetExistingTemplateContent<T>(IntentProjectItemTemplateBase<T> template)
        {
            var fileLocation = template.FileMetaData.GetFullLocationPathWithFileName();

            if (File.Exists(fileLocation))
            {
                return File.ReadAllText(fileLocation);
            }

            return null;
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
