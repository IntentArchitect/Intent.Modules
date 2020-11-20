using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ElementMappingModelExtensions
    {
        public static CriteriaSettings GetCriteriaSettings(this ElementMappingModel model)
        {
            var stereotype = model.GetStereotype("Criteria Settings");
            return stereotype != null ? new CriteriaSettings(stereotype) : null;
        }

        public static bool HasCriteriaSettings(this ElementMappingModel model)
        {
            return model.HasStereotype("Criteria Settings");
        }

        public static OutputSettings GetOutputSettings(this ElementMappingModel model)
        {
            var stereotype = model.GetStereotype("Output Settings");
            return stereotype != null ? new OutputSettings(stereotype) : null;
        }

        public static bool HasOutputSettings(this ElementMappingModel model)
        {
            return model.HasStereotype("Output Settings");
        }


        public class CriteriaSettings
        {
            private IStereotype _stereotype;

            public CriteriaSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement FromType()
            {
                return _stereotype.GetProperty<IElement>("From Type");
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

        public class OutputSettings
        {
            private IStereotype _stereotype;

            public OutputSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ChildMappingModeOptions ChildMappingMode()
            {
                return new ChildMappingModeOptions(_stereotype.GetProperty<string>("Child Mapping Mode"));
            }

            public IElement ToType()
            {
                return _stereotype.GetProperty<IElement>("To Type");
            }

            public IElement UseMappingSettings()
            {
                return _stereotype.GetProperty<IElement>("Use Mapping Settings");
            }

            public class ChildMappingModeOptions
            {
                public readonly string Value;

                public ChildMappingModeOptions(string value)
                {
                    Value = value;
                }

                public bool IsMapToType()
                {
                    return Value == "Map to Type";
                }
                public bool IsTraverse()
                {
                    return Value == "Traverse";
                }
            }

        }

    }
}