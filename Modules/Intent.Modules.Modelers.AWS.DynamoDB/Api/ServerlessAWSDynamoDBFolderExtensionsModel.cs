using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ServerlessAWSDynamoDBFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerlessAWSDynamoDBFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<DynamoDBModel> DynamoDBs => _element.ChildElements
            .GetElementsOfType(DynamoDBModel.SpecializationTypeId)
            .Select(x => new DynamoDBModel(x))
            .ToList();

    }
}