using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Merge)]
    public class EventingPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Eventing Package";
        public const string SpecializationTypeId = "df96d537-7bb5-4c49-811f-973fa6e95beb";

        [IntentManaged(Mode.Ignore)]
        public EventingPackageModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<MessageModel> Messages => UnderlyingPackage.ChildElements
            .GetElementsOfType(MessageModel.SpecializationTypeId)
            .Select(x => new MessageModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => UnderlyingPackage.ChildElements
            .GetElementsOfType(EnumModel.SpecializationTypeId)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<ConsumerModel> Consumers => UnderlyingPackage.ChildElements
            .GetElementsOfType(ConsumerModel.SpecializationTypeId)
            .Select(x => new ConsumerModel(x))
            .ToList();

        public IList<ProducerModel> Producers => UnderlyingPackage.ChildElements
            .GetElementsOfType(ProducerModel.SpecializationTypeId)
            .Select(x => new ProducerModel(x))
            .ToList();

    }
}