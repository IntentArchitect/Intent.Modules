﻿using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.CSharpReceivingProxy
{
    partial class CSharpIpcReceivingProxyTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, ITemplatePostCreationHook, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Electron.IpcProxy.CSharpIpcReceivingProxy";

        public CSharpIpcReceivingProxyTemplate(IServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
            AddTypeSource(CSharpTypeSource.InProject(Project, DTOTemplate.IDENTIFIER, "List<{0}>"));
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Identifier, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}NodeIpcProxy",
                fileExtension: "cs",
                defaultLocationInProject: @"NodeIpcProxies/Generated",
                className: $"{Model.Name}NodeIpcProxy",
                @namespace: "${Project.ProjectName}");
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NewtonsoftJson,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        private string GetOperationCallParameters(IOperation o)
        {
            if (!o.Parameters.Any())
            {
                return string.Empty;
            }

            return o.Parameters
                .Select((x, i) => $"Deserialize<{GetTypeName(x.Type, "List<{0}>")}>(methodParameters[{i}])")
                .Aggregate((x, y) => x + ", " + y);
        }
    }
}