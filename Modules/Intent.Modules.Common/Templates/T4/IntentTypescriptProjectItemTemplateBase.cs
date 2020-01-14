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
        private readonly ICollection<ITemplateDependency> _detectedDependencies = new List<ITemplateDependency>();

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


        private readonly ICollection<IClassTypeSource> _classTypeSources = new List<IClassTypeSource>();

        public void AddTypeSource(string templateId, string collectionFormat = "IEnumerable<{0}>")
        {
            _classTypeSources.Add(CSharpTypeSource.InProject(Project, templateId, collectionFormat));
        }

        public void AddTypeSource(IClassTypeSource classTypeSource)
        {
            _classTypeSources.Add(classTypeSource);
        }

        private bool _onCreatedHasHappened;

        public override void OnCreated()
        {
            base.OnCreated();
            _onCreatedHasHappened = true;
            foreach (var classTypeSource in _classTypeSources)
            {
                Types.AddClassTypeSource(classTypeSource);
            }
        }

        public string GetTypeName(ITypeReference typeReference)
        {
            return Types.Get(typeReference);
        }

        public string GetTemplateClassName(string templateId, ITemplateDependency templateDependency)
        {
            if (!_onCreatedHasHappened)
            {
                throw new Exception($"${nameof(GetTemplateClassName)} cannot be called during template instantiation.");
            }

            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependency);
            if (template != null)
            {
                _detectedDependencies.Add(templateDependency);
                return template.ClassName;
            }
            throw new Exception($"Could not find template with Id: {templateId} for model {Model.ToString()}");
        }

        public string GetTemplateClassName(string templateId)
        {
            return GetTemplateClassName(templateId, TemplateDependency.OnTemplate(templateId));
        }

        public string GetTemplateClassName(string templateId, IMetadataModel model)
        {
            return GetTemplateClassName(templateId, TemplateDependency.OnModel(templateId, model));
        }

        public string GetTemplateClassName(string templateId, string modelId)
        {
            return GetTemplateClassName(templateId, TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId));
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

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return base.GetTemplateDependencies()
                .Concat(_detectedDependencies);
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