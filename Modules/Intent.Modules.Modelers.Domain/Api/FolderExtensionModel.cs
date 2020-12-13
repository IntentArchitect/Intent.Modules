using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
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
            .Where(x => x.SpecializationType == ClassModel.SpecializationType)
            .Select(x => new ClassModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => _element.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<CommentModel> Comments => _element.ChildElements
            .Where(x => x.SpecializationType == CommentModel.SpecializationType)
            .Select(x => new CommentModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => _element.ChildElements
            .Where(x => x.SpecializationType == DiagramModel.SpecializationType)
            .Select(x => new DiagramModel(x))
            .ToList();

    }
}