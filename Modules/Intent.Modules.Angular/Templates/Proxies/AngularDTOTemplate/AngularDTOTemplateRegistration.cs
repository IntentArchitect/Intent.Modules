using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AngularDTOTemplateRegistration : ModelTemplateRegistrationBase<IModuleDTOModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public AngularDTOTemplateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularDTOTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModuleDTOModel model)
        {
            return new AngularDTOTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IModuleDTOModel> GetModels(Engine.IApplication application)
        {
            var dtoModels = new List<ModuleDTOModel>();
            foreach (var moduleModel in _metaDataManager.GetModules(application))
            {
                var operations = moduleModel.ServiceProxies
                    .SelectMany(x => x.Operations).ToList();
                var classes = operations
                    .SelectMany(x => x.Parameters)
                    .Select(x => x.Type.Model)
                    .Concat(operations.Where(x => x.ReturnType != null).Select(x => x.ReturnType.Type.Model));

                dtoModels.AddRange(classes.Where(x => x.IsDTO()).Select(x => new ModuleDTOModel(x, moduleModel)).ToList());
            }

            return dtoModels;
        }
    }
}