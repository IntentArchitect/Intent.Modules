using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<CommentModel> GetCommentModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CommentModel.SpecializationTypeId)
                .Select(x => new CommentModel(x))
                .ToList();
        }

        public static IList<DiagramModel> GetDiagramModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DiagramModel.SpecializationTypeId)
                .Select(x => new DiagramModel(x))
                .ToList();
        }

        public static IList<IAMPolicyStatementModel> GetIAMPolicyStatementModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(IAMPolicyStatementModel.SpecializationTypeId)
                .Select(x => new IAMPolicyStatementModel(x))
                .ToList();
        }

        public static IList<MessageModel> GetMessageModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(MessageModel.SpecializationTypeId)
                .Select(x => new MessageModel(x))
                .ToList();
        }

        public static IList<PlaceholderModel> GetPlaceholderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(PlaceholderModel.SpecializationTypeId)
                .Select(x => new PlaceholderModel(x))
                .ToList();
        }

    }
}