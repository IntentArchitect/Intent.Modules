using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class MappingCriteriaExtensions
    {
        public static CriteriaSettings GetCriteriaSettings(this MappingCriteriaModel model)
        {
            var stereotype = model.GetStereotype("Criteria Settings");
            return stereotype != null ? new CriteriaSettings(stereotype) : null;
        }


        public class CriteriaSettings
        {
            private IStereotype _stereotype;

            public CriteriaSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public HasTypeReferenceOptions HasTypeReference()
            {
                return new HasTypeReferenceOptions(_stereotype.GetProperty<string>("Has Type-Reference"));
            }

            public HasChildrenOptions HasChildren()
            {
                return new HasChildrenOptions(_stereotype.GetProperty<string>("Has Children"));
            }

            public IsCollectionOptions IsCollection()
            {
                return new IsCollectionOptions(_stereotype.GetProperty<string>("Is Collection"));
            }

            public class HasTypeReferenceOptions
            {
                public readonly string Value;

                public HasTypeReferenceOptions(string value)
                {
                    Value = value;
                }

                public bool IsYes()
                {
                    return Value == "Yes";
                }
                public bool IsNo()
                {
                    return Value == "No";
                }
                public bool IsNotApplicable()
                {
                    return Value == "Not Applicable";
                }
            }

            public class HasChildrenOptions
            {
                public readonly string Value;

                public HasChildrenOptions(string value)
                {
                    Value = value;
                }

                public bool IsYes()
                {
                    return Value == "Yes";
                }
                public bool IsNo()
                {
                    return Value == "No";
                }
                public bool IsNotApplicable()
                {
                    return Value == "Not Applicable";
                }
            }

            public class IsCollectionOptions
            {
                public readonly string Value;

                public IsCollectionOptions(string value)
                {
                    Value = value;
                }

                public bool IsYes()
                {
                    return Value == "Yes";
                }
                public bool IsNo()
                {
                    return Value == "No";
                }
                public bool IsNotApplicable()
                {
                    return Value == "Not Applicable";
                }
            }

        }

    }
}