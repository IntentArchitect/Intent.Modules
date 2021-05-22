using System;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates
{
    [Obsolete("Use TypeScriptTemplateBase")]
    public abstract class IntentTypescriptProjectItemTemplateBase<TModel> : IntentTemplateBase<TModel>, IClassProvider
    {
        public IntentTypescriptProjectItemTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        public string Namespace
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"];
                }
                return null;
            }
        }

        public string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"];
                }
                return FileMetadata.FileName;
            }
        }

        public string TypeName => string.IsNullOrWhiteSpace(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

        public string Location => FileMetadata.LocationInProject;

        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public new void AddTypeSource(string templateId, string collectionFormat = "{0}[]")
        {
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, templateId, collectionFormat));
        }

        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();

            return templateOutput;
//            return $@"{DependencyImports}
//{templateOutput}";
        }
    }

    //public static class PathExtensions
    //{
    //    public static string GetRelativePath(this string from, string to)
    //    {
    //        var url = new Uri("http://localhost/" + to, UriKind.Absolute);
    //        var relativeUrl = new Uri("http://localhost/" + from, UriKind.Absolute).MakeRelativeUri(url);
    //        return "./" + relativeUrl.ToString();
    //    }
    //}
}