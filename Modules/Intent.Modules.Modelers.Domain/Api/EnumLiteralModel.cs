using System.Collections.Generic;
using Intent.Metadata.Models;
using System;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EnumLiteralModel : IHasStereotypes, IMetadataModel
    {
        private readonly ILiteral _literal;

        public EnumLiteralModel(ILiteral literal)
        {
            _literal = literal;
        }

        public string Id => _literal.Id;
        public IEnumerable<IStereotype> Stereotypes => _literal.Stereotypes;
        public string Name => _literal.Name;
        public string Value => _literal.Value;
        public string Comment => _literal.Comment;
        protected readonly IElement _element;
        public const string SpecializationType = "Enum-Literal";

        [IntentManaged(Mode.Fully)]
        public bool Equals(EnumLiteralModel other)
        {
            return Equals(_element, other._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnumLiteralModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
    }
}