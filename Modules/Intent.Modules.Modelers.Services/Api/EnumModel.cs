using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EnumModel
        : IHasStereotypes, IMetadataModel
    {
        private readonly IElement _element;

        public EnumModel(IElement element)
        {
            _element = element;
            Folder = Api.FolderModel.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }
        public string Name => _element.Name;
        public IElementApplication Application => _element.Application;

        [IntentManaged(Mode.Fully)]
        public IList<EnumLiteralModel> Literals => _element.ChildElements
            .Where(x => x.SpecializationType == Api.EnumLiteralModel.SpecializationType)
            .Select(x => new EnumLiteralModel(x))
            .ToList<EnumLiteralModel>();
        public string Comment => _element.Comment;
        public const string SpecializationType = "Enum";

        protected bool Equals(EnumModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnumModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}