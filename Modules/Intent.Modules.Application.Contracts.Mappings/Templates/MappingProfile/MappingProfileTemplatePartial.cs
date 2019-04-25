﻿using System;
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
using Intent.Modelers.Domain.Api;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    partial class MappingProfileTemplate : IntentRoslynProjectItemTemplateBase<IList<IDTOModel>>, IBeforeTemplateExecutionHook, IPostTemplateCreation
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

        public MappingProfileTemplate(IProject project, IList<IDTOModel> model)
            : base(IDENTIFIER, project, model)
        {
        }

        public void Created()
        {
            _domainTemplateDependancyConfigValue = GetMetaData().CustomMetaData[DomainTemplateDependancyConfigId];
            _stereotypeNameConfigValue = GetMetaData().CustomMetaData[StereotypeNameConfigId];
            _stereotypeTypePropertyConfigValue = GetMetaData().CustomMetaData[StereotypeTypePropertyConfigId];
            _stereotypeNamespacePropertyConfigValue = GetMetaData().CustomMetaData[StereotypeNamespacePropertyConfigId];
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new[] { NugetPackages.AutoMapper });
        }

        public string GetContractType(IDTOModel model)
        {
            var templateDependancy = TemplateDependency.OnModel<IDTOModel>(GetMetaData().CustomMetaData[ContractTemplateDependancyConfigId], (to) => to.Id == model.Id);
            var templateOutput = Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {Id} Depends on : {ContractTemplateDependancyConfigId} Model : {model.Id}");
            }
            return templateOutput.FullTypeName();
        }

        public string GetSourceType(IDTOModel model)
        {
            var type = model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeTypePropertyConfigValue);
            var @namespace = model.GetStereotypeProperty<string>(_stereotypeNameConfigValue, _stereotypeNamespacePropertyConfigValue);

            if (!string.IsNullOrWhiteSpace(type))
            {
                return !string.IsNullOrWhiteSpace(@namespace)
                    ? $"{@namespace}.{type}"
                    : type;
            }

            var templateDependancy = TemplateDependency.OnModel<IClass>(_domainTemplateDependancyConfigValue, (to) => to.Id == model.MappedClass.ClassId);
            var templateOutput = Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
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
                    $"Then tried finding an instance of template with ID '{_domainTemplateDependancyConfigValue}' and model ID of {model.MappedClass.ClassId}, but none was found." +
                    $"\r\n");
            }
            return templateOutput.FullTypeName();
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "DtoProfile",
                fileExtension: "cs",
                defaultLocationInProject: "Mappings",
                className: "DtoProfile",
                @namespace: "${Project.ProjectName}"
                );
        }

        public void BeforeTemplateExecution()
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

        private string ToPascalCasePath(string path)
        {
            var piecies = path.Split('.');
            return piecies.Select(x => x.ToPascalCase()).Aggregate((x, y) => x + "." + y);
        }
    }
}
