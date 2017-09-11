using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Packages.Application.Contracts.Templates.ServiceContract;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Packages.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    [Description("Intent Application - Service Call Handler")]
    public class Registrations : IProjectTemplateRegistration
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => ServiceCallHandlerImplementationTemplate.Identifier;


        private ITemplate CreateTemplateInstance(IProject project, IServiceModel serviceModel, IOperationModel operationModel)
        {
            return new ServiceCallHandlerImplementationTemplate(project, serviceModel, operationModel);
        }

        public void DoRegistration(Action<string, Func<IProject, ITemplate>> register, IApplication applicationManager)
        {
            var serviceModels = _metaDataManager.GetMetaData<IServiceModel>(new MetaDataType("Service")).Where(x => x.Application.Name == applicationManager.ApplicationName).ToList();
            foreach (var serviceModel in serviceModels)
            {
                foreach (var operationModel in serviceModel.Operations)
                {
                    register(TemplateId, (project) => CreateTemplateInstance(project, serviceModel, operationModel));
                }
            }
        }
    }
}

