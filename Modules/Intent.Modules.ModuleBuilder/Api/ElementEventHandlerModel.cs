using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementEventHandlerModel : ScriptModel, IHasStereotypes, IMetadataModel
    {
        public ElementEventHandlerModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementEventHandlerModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementEventHandlerModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public ElementMacroPersistable ToPersistable()
        {
            return new ElementMacroPersistable()
            {
                Trigger = Name.Trim().Replace(" ", "").ToKebabCase(),
                Script = Script
            };
        }
        public new const string SpecializationType = "Element Event Handler";
        public new const string SpecializationTypeId = "0ef412e3-d15a-45d3-bcd3-f646165f2eb6";
    }
}