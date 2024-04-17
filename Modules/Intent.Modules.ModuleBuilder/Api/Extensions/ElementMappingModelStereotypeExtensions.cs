using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ElementMappingModelStereotypeExtensions
    {
        public static BehaviourSettings GetBehaviourSettings(this ElementMappingModel model)
        {
            var stereotype = model.GetStereotype(BehaviourSettings.DefinitionId);
            return stereotype != null ? new BehaviourSettings(stereotype) : null;
        }

        public static bool HasBehaviourSettings(this ElementMappingModel model)
        {
            return model.HasStereotype(BehaviourSettings.DefinitionId);
        }

        public static bool TryGetBehaviourSettings(this ElementMappingModel model, out BehaviourSettings stereotype)
        {
            if (!HasBehaviourSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new BehaviourSettings(model.GetStereotype(BehaviourSettings.DefinitionId));
            return true;
        }

        public static CriteriaSettings GetCriteriaSettings(this ElementMappingModel model)
        {
            var stereotype = model.GetStereotype(CriteriaSettings.DefinitionId);
            return stereotype != null ? new CriteriaSettings(stereotype) : null;
        }

        public static bool HasCriteriaSettings(this ElementMappingModel model)
        {
            return model.HasStereotype(CriteriaSettings.DefinitionId);
        }

        public static bool TryGetCriteriaSettings(this ElementMappingModel model, out CriteriaSettings stereotype)
        {
            if (!HasCriteriaSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CriteriaSettings(model.GetStereotype(CriteriaSettings.DefinitionId));
            return true;
        }

        public static OutputSettings GetOutputSettings(this ElementMappingModel model)
        {
            var stereotype = model.GetStereotype(OutputSettings.DefinitionId);
            return stereotype != null ? new OutputSettings(stereotype) : null;
        }

        public static bool HasOutputSettings(this ElementMappingModel model)
        {
            return model.HasStereotype(OutputSettings.DefinitionId);
        }

        public static bool TryGetOutputSettings(this ElementMappingModel model, out OutputSettings stereotype)
        {
            if (!HasOutputSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OutputSettings(model.GetStereotype(OutputSettings.DefinitionId));
            return true;
        }


        public class BehaviourSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "47ec6487-1f69-4691-9c8c-031e2da08c07";

            public BehaviourSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool AutoSelectChildren()
            {
                return _stereotype.GetProperty<bool>("Auto-select Children");
            }

            public class AutoSelectChildrenOptions
            {
                public readonly string Value;

                public AutoSelectChildrenOptions(string value)
                {
                    Value = value;
                }

                public AutoSelectChildrenOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Yes":
                            return AutoSelectChildrenOptionsEnum.Yes;
                        case "No":
                            return AutoSelectChildrenOptionsEnum.No;
                        case "Not Applicable":
                            return AutoSelectChildrenOptionsEnum.NotApplicable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum AutoSelectChildrenOptionsEnum
            {
                Yes,
                No,
                NotApplicable
            }
        }

        public class CriteriaSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c677f491-8290-47ee-9e98-4c26bc76b592";

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

            public string FilterFunction()
            {
                return _stereotype.GetProperty<string>("Filter Function");
            }

            public class HasTypeReferenceOptions
            {
                public readonly string Value;

                public HasTypeReferenceOptions(string value)
                {
                    Value = value;
                }

                public HasTypeReferenceOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Yes":
                            return HasTypeReferenceOptionsEnum.Yes;
                        case "No":
                            return HasTypeReferenceOptionsEnum.No;
                        case "Not Applicable":
                            return HasTypeReferenceOptionsEnum.NotApplicable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum HasTypeReferenceOptionsEnum
            {
                Yes,
                No,
                NotApplicable
            }
            public class HasChildrenOptions
            {
                public readonly string Value;

                public HasChildrenOptions(string value)
                {
                    Value = value;
                }

                public HasChildrenOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Yes":
                            return HasChildrenOptionsEnum.Yes;
                        case "No":
                            return HasChildrenOptionsEnum.No;
                        case "Not Applicable":
                            return HasChildrenOptionsEnum.NotApplicable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum HasChildrenOptionsEnum
            {
                Yes,
                No,
                NotApplicable
            }
            public class IsCollectionOptions
            {
                public readonly string Value;

                public IsCollectionOptions(string value)
                {
                    Value = value;
                }

                public IsCollectionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Yes":
                            return IsCollectionOptionsEnum.Yes;
                        case "No":
                            return IsCollectionOptionsEnum.No;
                        case "Not Applicable":
                            return IsCollectionOptionsEnum.NotApplicable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum IsCollectionOptionsEnum
            {
                Yes,
                No,
                NotApplicable
            }
        }

        public class OutputSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "740ed66f-c70d-45b0-80d5-6f663da53ed5";

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

                public ChildMappingModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Map to Type":
                            return ChildMappingModeOptionsEnum.MapToType;
                        case "Traverse":
                            return ChildMappingModeOptionsEnum.Traverse;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum ChildMappingModeOptionsEnum
            {
                MapToType,
                Traverse
            }
        }

    }
}