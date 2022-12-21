using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully)]
    public class DynamoDBModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "DynamoDB";
        public const string SpecializationTypeId = "8b6ad332-78da-4e2b-9a94-a9dc44a32f65";

        [IntentManaged(Mode.Ignore)]
        public DynamoDBModel(IPackage package)
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

        public IList<DynamoDBTableModel> DynamoDBTables => UnderlyingPackage.ChildElements
    .GetElementsOfType(DynamoDBTableModel.SpecializationTypeId)
    .Select(x => new DynamoDBTableModel(x))
    .ToList();

    }
}