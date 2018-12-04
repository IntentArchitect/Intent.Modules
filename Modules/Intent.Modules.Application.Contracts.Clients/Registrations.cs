using Intent.MetaModel.Common;
using Intent.MetaModel.Dto.Old;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.TypeResolution;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.Application.Contracts.Clients
{
    public class TemplateIds
    {
        public const string ClientDTO = "Intent.Application.Contracts.DTO.Client";
        public const string ClientServiceContract = "Intent.Application.Contracts.ServiceContract.Client";
        public const string ClientDtoLegacy = "Intent.Application.Contracts.DTO.Legacy.Client";
        public const string ClientLegacyServiceContractLegacy = "Intent.Application.Contracts.ServiceContract.Legacy.Client";
    }

    [Description("Intent Applications Service Contracts (Clients)")]
    public class ServiceContractRegistrations : ModelTemplateRegistrationBase<MetaModel.Service.IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ServiceContractRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientServiceContract;

        public override ITemplate CreateTemplateInstance(IProject project, MetaModel.Service.IServiceModel model)
        {
            return new Templates.ServiceContract.ServiceContractTemplate(project, model, TemplateId);
        }

        public override IEnumerable<MetaModel.Service.IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<MetaModel.Service.IServiceModel>("Services");
            if (!serviceModels.Any())
            {
                serviceModels = _metaDataManager.GetMetaData<MetaModel.Service.IServiceModel>("Service"); // backward compatibility
            }
            return serviceModels
                .Where(x => x.GetConsumers().Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }

    [Description("Intent Applications Contracts DTO")]
    public class DtoRegistrations : ModelTemplateRegistrationBase<MetaModel.DTO.IDTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DtoRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientDTO;

        public override ITemplate CreateTemplateInstance(IProject project, MetaModel.DTO.IDTOModel model)
        {
            return new Templates.DTO.DTOTemplate(project, model, TemplateId);
        }

        public override IEnumerable<MetaModel.DTO.IDTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metaDataManager.GetMetaData<MetaModel.DTO.IDTOModel>(new MetaDataIdentifier("Services"));
            if (!dtoModels.Any())
            {
                dtoModels = _metaDataManager.GetMetaData<MetaModel.DTO.IDTOModel>(new MetaDataIdentifier("DTO")); // backward compatibility
            }
            return dtoModels
                .Where(x => x.GetConsumers().Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
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
        public static string GetQualifiedName<T>(this ITypeReference typeInfo, T template)
            where T: IProjectItemTemplate, IRequireTypeResolver
        {
            return typeInfo.GetQualifiedName(template, TemplateIds.ClientDTO);
        }
    }

    public static class DTOModelExtensions
    {
        public static IEnumerable<string> GetConsumers<T>(this T dto) where T: IHasFolder, IHasStereotypes
        {
            if (dto.HasStereotype("Consumers"))
            {
                return dto.GetStereotypeProperty("Consumers", "CommaSeperatedList", "").Split(',').Select(x => x.Trim());
            }
            return dto.GetStereotypeInFolders("Consumers").GetProperty("CommaSeperatedList", "").Split(',').Select(x => x.Trim());
        }
    }
}
