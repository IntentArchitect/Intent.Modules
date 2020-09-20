using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Ignore, Signature = Mode.Merge)]
    public class ModuleDTOModel : DTOModel, IHasStereotypes, IMetadataModel
    {
        public new const string SpecializationType = "Module DTO";

        public ModuleDTOModel(IElement element, ModuleModel module) : base(element)
        {
            Module = module;
        }

        public ModuleModel Module { get; }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ModuleDTOModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleDTOModel)obj);
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