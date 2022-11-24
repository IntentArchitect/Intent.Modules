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
    public class DynamoDBFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<DynamoDBTableModel> DynamoDBTables => _element.ChildElements
            .GetElementsOfType(DynamoDBTableModel.SpecializationTypeId)
            .Select(x => new DynamoDBTableModel(x))
            .ToList();

    }
}