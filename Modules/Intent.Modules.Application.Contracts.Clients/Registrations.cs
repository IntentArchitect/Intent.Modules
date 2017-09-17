using Intent.MetaModel.Common;
using Intent.MetaModel.Dto.Old;
using Intent.Modules.Application.Contracts.Legacy.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;
using ServiceContractTemplate = Intent.Modules.Application.Contracts.Legacy.ServiceContract.ServiceContractTemplate;

namespace Intent.Modules.Application.Contracts.Clients
{
    public class TemplateIds
    {
        public const string ClientDTO = "Intent.Application.Contracts.DTO.Client";
        public const string ClientServiceContract = "Intent.Application.Contracts.ServiceContract.Client";
        public const string ClientDtoLegacy = "Intent.Application.Contracts.DTO.Legacy.Client";
        public const string ClientLegacyServiceContractLegacy = "Intent.Application.Contracts.ServiceContract.Legacy.Client";
    }

    //public class DecoratorIds
    //{
    //    public const string ClientDataContractDecorator = "Intent.Application.Contracts.DataContractDecorator";
    //}

    public class LegacyRegistrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var dtoModels = metaDataManager
                .GetMetaData<DtoModel>(new MetaDataType("DtoProjection"))
                .Where(x => x.Clients.Any(y => application.ApplicationName.Equals(y, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var serviceModels = metaDataManager
                .GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy"))
                .Where(x => x.Clients.Any(y => application.ApplicationName.Equals(y, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var model in dtoModels)
            {
                RegisterTemplate(TemplateIds.ClientDtoLegacy, project => new DTOTemplate(project, model, TemplateIds.ClientDtoLegacy));
            }

            foreach (var serviceModel in serviceModels)
            {
                RegisterTemplate(TemplateIds.ClientLegacyServiceContractLegacy, project => new ServiceContractTemplate(serviceModel, project, TemplateIds.ClientLegacyServiceContractLegacy));
            }
        }
    }

    [Description("Intent Applications Service Contracts (Clients)")]
    public class ServiceContractRegistrations : ModelTemplateRegistrationBase<MetaModel.Service.ServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ServiceContractRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientServiceContract;

        public override ITemplate CreateTemplateInstance(IProject project, MetaModel.Service.ServiceModel model)
        {
            return new Templates.ServiceContract.ServiceContractTemplate(project, model, TemplateId, TemplateIds.ClientDTO);
        }

        public override IEnumerable<MetaModel.Service.ServiceModel> GetModels(IApplication application)
        {
            return _metaDataManager
                .GetMetaData<MetaModel.Service.ServiceModel>(new MetaDataType("Service"))
                .Where(x => x.GetPropertyValue("Consumers", "CommaSeperatedList", "").Split(',').Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }

    [Description("Intent Applications Contracts DTO")]
    public class DtoRegistrations : ModelTemplateRegistrationBase<MetaModel.DTO.DTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DtoRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientDTO;

        public override ITemplate CreateTemplateInstance(IProject project, MetaModel.DTO.DTOModel model)
        {
            return new Templates.DTO.DTOTemplate(project, model, TemplateId);
        }

        public override IEnumerable<MetaModel.DTO.DTOModel> GetModels(IApplication application)
        {
            return _metaDataManager
                .GetMetaData<MetaModel.DTO.DTOModel>(new MetaDataType("DTO"))
                .Where(x => x.GetPropertyValue("Consumers", "CommaSeperatedList", "").Split(',').Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }

    // JL: Don't think we can have this seperate, it's probably not needed either anyway
    //[Description("Intent Applications Contracts DTO - Data Contract decorator (Clients)")]
    //public class DataContractDTOAttributeDecoratorRegistration : DecoratorRegistration<IDTOAttributeDecorator>
    //{
    //    public override string DecoratorId => DataContractDTOAttributeDecorator.Id;

    //    public override object CreateDecoratorInstance()
    //    {
    //        return new DataContractDTOAttributeDecorator();
    //    }
    //}

    public static class CSharpTypeReferenceExtensions
    {
        public static string GetQualifiedName(this ITypeReference typeInfo, IProjectItemTemplate template)
        {
            return typeInfo.GetQualifiedName(template, TemplateIds.ClientDTO);
        }
    }
}
