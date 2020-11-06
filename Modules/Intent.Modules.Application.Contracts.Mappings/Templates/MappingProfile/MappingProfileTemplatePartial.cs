using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using Intent.Utils;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    partial class MappingProfileTemplate : CSharpTemplateBase<IList<DTOModel>>, ITemplateBeforeExecutionHook, ITemplatePostCreationHook
    {
        public const string IDENTIFIER = "Intent.Application.Contracts.Mapping.Profile";

        private const string ContractTemplateDependancyConfigId = "ContractTemplateDependancyId";
        private const string DomainTemplateDependancyConfigId = "DomainTemplateDependancyId";
        private const string StereotypeNameConfigId = "Stereotype Name";
        private const string StereotypeTypePropertyConfigId = "Stereotype Property Name (Type)";
        private const string StereotypeNamespacePropertyConfigId = "Stereotype Property Name (Namespace)";

        private string _domainTemplateDependancyConfigValue;
        private string _stereotypeNameConfigValue;
        private string _stereotypeTypePropertyConfigValue;
        private string _stereotypeNamespacePropertyConfigValue;

        public MappingProfileTemplate(IProject project, IList<DTOModel> model)
            : base(IDENTIFIER, project, model)
        {
        }

        public override void OnCreated()
        {
            _domainTemplateDependancyConfigValue = GetMetadata().CustomMetadata[DomainTemplateDependancyConfigId];
            _stereotypeNameConfigValue = GetMetadata().CustomMetadata[StereotypeNameConfigId];
            _stereotypeTypePropertyConfigValue = GetMetadata().CustomMetadata[StereotypeTypePropertyConfigId];
            _stereotypeNamespacePropertyConfigValue = GetMetadata().CustomMetadata[StereotypeNamespacePropertyConfigId];
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new[] { NugetPackages.AutoMapper });
        }

        public string GetContractType(DTOModel model)
        {
            var templateDependancy = TemplateDependency.OnModel<DTOModel>(GetMetadata().CustomMetadata[ContractTemplateDependancyConfigId], (to) => to.Id == model.Id);
            var templateOutput = Project.Application.FindTemplateInstance<IClassProvider>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {Id} Depends on : {ContractTemplateDependancyConfigId} Model : {model.Id}");
            }
            return templateOutput.FullTypeName();
        }

        public string GetSourceType(DTOModel model)
        {
            var type = model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeTypePropertyConfigValue);
            var @namespace = model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeNamespacePropertyConfigValue);

            if (!string.IsNullOrWhiteSpace(type))
            {
                return !string.IsNullOrWhiteSpace(@namespace)
                    ? $"{@namespace}.{type}"
                    : type;
            }

            var templateDependancy = TemplateDependency.OnModel<ClassModel>(_domainTemplateDependancyConfigValue, (to) => to.Id == model.Mapping.ElementId);
            var templateOutput = Project.Application.FindTemplateInstance<IClassProvider>(templateDependancy);
            if (templateOutput == null)
            {
                throw new Exception(message:
                    $"\r\n" +
                    $"\r\n" +
                    $"Unable to resolve source type for DTO Mapping for '{model.Name}' ({model.Id}). " +
                    $"\r\n" +
                    $"\r\n" +
                    $"First tried checking on the DTO for existence of a stereotype '{_stereotypeNameConfigValue}' with populated property '{_stereotypeTypePropertyConfigValue}', but the stereotype and/or property was not present. " +
                    $"\r\n" +
                    $"\r\n" +
                    $"Then tried finding an instance of template with ID '{_domainTemplateDependancyConfigValue}' and model ID of {model.Mapping.ElementId}, but none was found." +
                    $"\r\n");
            }
            return templateOutput.FullTypeName();
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DtoProfile",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"{Namespace};" },
                { InitializationRequiredEvent.CallKey, "InitializeMapper();" },
                { InitializationRequiredEvent.MethodKey, $@"
        private void InitializeMapper()
        {{
           AutoMapper.Mapper.Initialize(x => x.AddProfile(new {ClassName}()));
        }}" },
                { InitializationRequiredEvent.TemplateDependencyIdKey, IDENTIFIER }
            });
        }

        public string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path
                .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
                .Select(x => x.Specialization == OperationModel.SpecializationType ? $"{x.Name.ToPascalCase()}()" : x.Name.ToPascalCase()));
        }
    }
}
