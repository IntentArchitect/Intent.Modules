using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ClassModel> GetClassModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ClassModel.SpecializationTypeId)
                .Select(x => new ClassModel(x))
                .ToList();
        }

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

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
                .Select(x => new TypeDefinitionModel(x))
                .ToList();
        }

    }
}