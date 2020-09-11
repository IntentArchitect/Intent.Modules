using System;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentTypescriptProjectItemTemplateBase<TModel> : IntentTemplateBase<TModel>, IHasClassDetails
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

        public string Location => FileMetadata.LocationInProject;

        public void AddTypeSource(string templateId, string collectionFormat = "{0}[]")
        {
            AddTypeSource(TypescriptTypeSource.InProject(ExecutionContext, templateId, collectionFormat));
        }


        public override string GetTemplateClassName(ITemplateDependency templateDependency, bool throwIfNotFound = true)
        {
            return FindTemplate<IHasClassDetails>(templateDependency, throwIfNotFound).ClassName;
        }

        //public string DependencyImports
        //{
        //    get
        //    {
        //        var sb = new StringBuilder();
        //        foreach (var dependency in GetTemplateDependencies().Select(Project.FindTemplateInstance<ITemplate>).Distinct())
        //        {
        //            var className = ((IHasClassDetails)dependency).ClassName;
        //            var location = GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(dependency.GetMetadata().GetRelativeFilePathWithFileName());
        //            sb.AppendLine($"import {{ {className} }} from '{location}';");
        //        }

        //        return sb.ToString();
        //    }
        //}

        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();

            return templateOutput;
//            return $@"{DependencyImports}
//{templateOutput}";
        }
    }

    public static class PathExtensions
    {
        public static string GetRelativePath(this string from, string to)
        {
            var url = new Uri("http://localhost/" + to, UriKind.Absolute);
            var relativeUrl = new Uri("http://localhost/" + from, UriKind.Absolute).MakeRelativeUri(url);
            return "./" + relativeUrl.ToString();
        }
    }
}