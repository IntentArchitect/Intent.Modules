using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Sql.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SqlTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public new const string SpecializationType = "Sql Template";

        public SqlTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(SqlTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SqlTemplateModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public new const string SpecializationTypeId = "f3e581f7-96eb-439f-8bc4-9103128f0bd7";
    }

    [IntentManaged(Mode.Fully)]
    public static class SqlTemplateModelExtensions
    {

        public static bool IsSqlTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SqlTemplateModel.SpecializationTypeId;
        }

        public static SqlTemplateModel AsSqlTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsSqlTemplateModel() ? new SqlTemplateModel((IElement)type) : null;
        }
    }
}