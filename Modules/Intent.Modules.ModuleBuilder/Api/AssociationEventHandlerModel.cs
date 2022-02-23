using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class AssociationEventHandlerModel : ScriptModel, IMetadataModel, IHasStereotypes, IHasName
    {
        public new const string SpecializationType = "Association Event Handler";
        public new const string SpecializationTypeId = "f9b3e6fd-3c51-4ed9-b214-cb7f5fc1cf7c";

        public AssociationEventHandlerModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Ignore)]
        public MacroPersistable ToPersistable()
        {
            return new MacroPersistable()
            {
                Trigger = Name.Trim().Replace(" ", "").ToKebabCase(),
                Script = Script
            };
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AssociationEventHandlerModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationEventHandlerModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationEventHandlerModelExtensions
    {

        public static bool IsAssociationEventHandlerModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationEventHandlerModel.SpecializationTypeId;
        }

        public static AssociationEventHandlerModel AsAssociationEventHandlerModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationEventHandlerModel() ? new AssociationEventHandlerModel((IElement)type) : null;
        }
    }
}