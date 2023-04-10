using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<ClassModel> Classes => _element.ChildElements
            .GetElementsOfType(ClassModel.SpecializationTypeId)
            .Select(x => new ClassModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => _element.ChildElements
            .GetElementsOfType(EnumModel.SpecializationTypeId)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<CommentModel> Comments => _element.ChildElements
            .GetElementsOfType(CommentModel.SpecializationTypeId)
            .Select(x => new CommentModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => _element.ChildElements
            .GetElementsOfType(DiagramModel.SpecializationTypeId)
            .Select(x => new DiagramModel(x))
            .ToList();

        public IList<DataContractModel> DomainObjects => _element.ChildElements
            .GetElementsOfType(DataContractModel.SpecializationTypeId)
            .Select(x => new DataContractModel(x))
            .ToList();

    }
}