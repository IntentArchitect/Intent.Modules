using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    partial class MappingTemplate : IntentRoslynProjectItemTemplateBase<DTOModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, ITemplatePostCreationHook, IHasDecorators<IMappingTemplateDecorator>
    {
        public const string Identifier = "Intent.Application.Contracts.Mapping";

        private const string ContractTemplateDependancyConfigId = "ContractTemplateDependancyId";
        private const string DomainTemplateDependancyConfigId = "DomainTemplateDependancyId";
        private const string StereotypeNameConfigId = "Stereotype Name";
        private const string StereotypeTypePropertyConfigId = "Stereotype Property Name (Type)";
        private const string StereotypeNamespacePropertyConfigId = "Stereotype Property Name (Namespace)";

        private string _domainTemplateDependancyConfigValue;
        private string _stereotypeNameConfigValue;
        private string _stereotypeTypePropertyConfigValue;
        private string _stereotypeNamespacePropertyConfigValue;
        private ITemplateDependency _domainTemplateDependancy;
        private ITemplateDependency _contractTemplateDependancy;
        private IList<IMappingTemplateDecorator> _decorators = new List<IMappingTemplateDecorator>();

        public MappingTemplate(IProject project, DTOModel model)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            _domainTemplateDependancyConfigValue = GetMetadata().CustomMetadata[DomainTemplateDependancyConfigId];
            _stereotypeNameConfigValue = GetMetadata().CustomMetadata[StereotypeNameConfigId];
            _stereotypeTypePropertyConfigValue = GetMetadata().CustomMetadata[StereotypeTypePropertyConfigId];
            _stereotypeNamespacePropertyConfigValue = GetMetadata().CustomMetadata[StereotypeNamespacePropertyConfigId];
            _contractTemplateDependancy = TemplateDependency.OnModel<DTOModel>(GetMetadata().CustomMetadata[ContractTemplateDependancyConfigId], (to) => to.Id == Model.Id);
            _domainTemplateDependancy = TemplateDependency.OnModel<ClassModel>(_domainTemplateDependancyConfigValue, (to) => to.Id == Model.MappedClass.ElementId);
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new INugetPackageInfo[] { NugetPackages.AutoMapper });
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[] { _contractTemplateDependancy, _domainTemplateDependancy };
        }

        public void AddDecorator(IMappingTemplateDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IMappingTemplateDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string ContractTypeName => Project.Application.FindTemplateInstance<IHasClassDetails>(_contractTemplateDependancy)?.ClassName ?? Model.Name;

        public string SourceTypeName
        {
            get
            {
                var type = Model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeTypePropertyConfigValue);
                var @namespace = Model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeNamespacePropertyConfigValue);

                if (!string.IsNullOrWhiteSpace(type))
                {
                    return !string.IsNullOrWhiteSpace(@namespace)
                        ? $"{@namespace}.{type}"
                        : type;
                }

                return
                    Project.Application.FindTemplateInstance<IHasClassDetails>(_domainTemplateDependancy)?.ClassName ??
                    throw new Exception(message:
                        $"\r\n" +
                        $"\r\n" +
                        $"Unable to resolve source type for DTO Mapping for '{Model.Name}' ({Model.Id}). " +
                        $"\r\n" +
                        $"\r\n" +
                        $"First tried checking on the DTO for existence of a stereotype '{_stereotypeNameConfigValue}' with populated property '{_stereotypeTypePropertyConfigValue}', but the stereotype and/or property was not present. " +
                        $"\r\n" +
                        $"\r\n" +
                        $"Then tried finding an instance of template with ID '{_domainTemplateDependancyConfigValue}' and model ID of {Model.MappedClass.ElementId}, but none was found." +
                        $"\r\n");
            }
        }

        public string DecoratorUsings
        {
            get
            {
                if (!GetDecorators().Any())
                {
                    return string.Empty;
                }

                return GetDecorators()
                    .SelectMany(x => x.Usings())
                    .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");
            }
        }

        public string GetDecoratorMembers(string contractTypeName, string domainTypeName)
        {
            if (!GetDecorators().Any())
            {
                return string.Empty;
            }

            return GetDecorators()
                .SelectMany(x => x.AdditionalMembers(contractTypeName, domainTypeName))
                .Select(x => $"{Environment.NewLine}{x}")
                .Aggregate((x, y) => $"{x}{y}");
        }

        public string StereotypedNamespaceBasedPath => string.Join("/", GetNamespaceParts());

        public string StereotypedNamespace => string.Join(".", GetNamespaceParts());
        public string SlashIfInNamespace => !string.IsNullOrWhiteSpace(StereotypedNamespaceBasedPath) ? "/" : string.Empty;

        public string DotIfInNamespace => !string.IsNullOrWhiteSpace(StereotypedNamespaceBasedPath) ? "." : string.Empty;

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Mapping",
                fileExtension: "cs",
                defaultLocationInProject: "Mappings${SlashIfInNamespace}${StereotypedNamespaceBasedPath}",
                className: "${Model.Name}Mapping",
                @namespace: "${Project.ProjectName}${DotIfInNamespace}${StereotypedNamespace}");
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath(includePackage: false)
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace"))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
    }
}
