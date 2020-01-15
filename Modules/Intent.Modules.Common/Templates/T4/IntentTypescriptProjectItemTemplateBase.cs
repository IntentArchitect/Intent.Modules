using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
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
                return this.Project.Name;
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

        public string DependencyImports
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var dependency in GetTemplateDependencies().Select(x => Project.FindTemplateInstance<ITemplate>(x)).Distinct())
                {
                    var className = ((IHasClassDetails)dependency).ClassName;
                    var location = GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(dependency.GetMetadata().GetRelativeFilePathWithFileName());
                    sb.AppendLine($"import {{ {className} }} from '{location}';");
                }

                return sb.ToString();
            }
        }

        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();

            return $@"{DependencyImports}
{templateOutput}";
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