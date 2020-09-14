using System;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    [Obsolete]
    public abstract class IntentTypescriptProjectItemTemplateBase<TModel> : IntentProjectItemTemplateBase<TModel>, IHasClassDetails
    {
        public IntentTypescriptProjectItemTemplateBase(string templateId, IProject project, TModel model) : base(templateId, project, model)
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
            AddTypeSource(TypescriptTypeSource.InProject(Project, templateId, collectionFormat));
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

    public class TypescriptDefaultFileMetadata : DefaultFileMetadata
    {
        public TypescriptDefaultFileMetadata(
                    OverwriteBehaviour overwriteBehaviour,
                    string codeGenType,
                    string fileName,
                    string fileExtension,
                    string defaultLocationInProject,
                    string className,
                    string @namespace = null,
                    string dependsUpon = null
                    )
            : base(overwriteBehaviour: overwriteBehaviour, 
                  codeGenType: codeGenType, 
                  fileName: fileName, 
                  fileExtension: fileExtension,
                  defaultLocationInProject: defaultLocationInProject)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                this.CustomMetadata["ClassName"] = className;
            }
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                this.CustomMetadata["Namespace"] = @namespace;
            }
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                this.CustomMetadata["Depends On"] = dependsUpon;
            }
        }
    }
}