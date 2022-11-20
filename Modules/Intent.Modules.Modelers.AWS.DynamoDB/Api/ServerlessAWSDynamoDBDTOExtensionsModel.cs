using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Serverless.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ServerlessAWSDynamoDBDTOExtensionsModel : DTOModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerlessAWSDynamoDBDTOExtensionsModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class ServerlessAWSDynamoDBDTOExtensionsModelExtensions
    {

        public static bool HasMapFromDynamoDBTableItemMapping(this DTOModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "55ed7b49-b077-405f-a8e2-9ad79953d35e";
        }

        public static IElementMapping GetMapFromDynamoDBTableItemMapping(this DTOModel type)
        {
            return type.HasMapFromDynamoDBTableItemMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}