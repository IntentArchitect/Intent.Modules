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
    public static class StaticMappableSettingsModelStereotypeExtensions
    {
        public static MappableSettings GetMappableSettings(this StaticMappableSettingsModel model)
        {
            var stereotype = model.GetStereotype(MappableSettings.DefinitionId);
            return stereotype != null ? new MappableSettings(stereotype) : null;
        }


        public static bool HasMappableSettings(this StaticMappableSettingsModel model)
        {
            return model.HasStereotype(MappableSettings.DefinitionId);
        }

        public static bool TryGetMappableSettings(this StaticMappableSettingsModel model, out MappableSettings stereotype)
        {
            if (!HasMappableSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappableSettings(model.GetStereotype(MappableSettings.DefinitionId));
            return true;
        }

        public class MappableSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "df24679d-09ec-4efc-af37-a1a926b7a108";

            public MappableSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public RepresentsOptions Represents()
            {
                return new RepresentsOptions(_stereotype.GetProperty<string>("Represents"));
            }

            public IIconModel IconOverride()
            {
                return _stereotype.GetProperty<IIconModel>("Icon Override");
            }

            public string DisplayFunction()
            {
                return _stereotype.GetProperty<string>("Display Function");
            }

            public string FilterFunction()
            {
                return _stereotype.GetProperty<string>("Filter Function");
            }

            public string IsRequiredFunction()
            {
                return _stereotype.GetProperty<string>("Is Required Function");
            }

            public string IsMappableFunction()
            {
                return _stereotype.GetProperty<string>("Is Mappable Function");
            }

            public bool AllowMultipleMappings()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple Mappings");
            }

            public TraversableModeOptions TraversableMode()
            {
                return new TraversableModeOptions(_stereotype.GetProperty<string>("Traversable Mode"));
            }

            public IElement[] TraversableTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Traversable Types") ?? new IElement[0];
            }

            public string OverrideTypeReferenceFunction()
            {
                return _stereotype.GetProperty<string>("Override Type Reference Function");
            }

            public string GetTraversableTypeFunction()
            {
                return _stereotype.GetProperty<string>("Get Traversable Type Function");
            }

            public SyncMappingToOptions SyncMappingTo()
            {
                return new SyncMappingToOptions(_stereotype.GetProperty<string>("Sync Mapping To"));
            }

            public IStereotypeDefinition SyncStereotype()
            {
                return _stereotype.GetProperty<IStereotypeDefinition>("Sync Stereotype");
            }

            public string SyncStereotypeProperty()
            {
                return _stereotype.GetProperty<string>("Sync Stereotype Property");
            }

            public bool CanBeModified()
            {
                return _stereotype.GetProperty<bool>("Can Be Modified");
            }

            public string CreateNameFunction()
            {
                return _stereotype.GetProperty<string>("Create Name Function");
            }

            public IElement UseChildMappingsFrom()
            {
                return _stereotype.GetProperty<IElement>("Use Child Mappings From");
            }

            public string OnMappingChangedScript()
            {
                return _stereotype.GetProperty<string>("On Mapping Changed Script");
            }

            public string ValidateFunction()
            {
                return _stereotype.GetProperty<string>("Validate Function");
            }

            public class RepresentsOptions
            {
                public readonly string Value;

                public RepresentsOptions(string value)
                {
                    Value = value;
                }

                public RepresentsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Data":
                            return RepresentsOptionsEnum.Data;
                        case "Invokable":
                            return RepresentsOptionsEnum.Invokable;
                        case "Class":
                            return RepresentsOptionsEnum.Class;
                        case "Event":
                            return RepresentsOptionsEnum.Event;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsData()
                {
                    return Value == "Data";
                }
                public bool IsInvokable()
                {
                    return Value == "Invokable";
                }
                public bool IsClass()
                {
                    return Value == "Class";
                }
                public bool IsEvent()
                {
                    return Value == "Event";
                }
            }

            public enum RepresentsOptionsEnum
            {
                Data,
                Invokable,
                Class,
                Event
            }
            public class TraversableModeOptions
            {
                public readonly string Value;

                public TraversableModeOptions(string value)
                {
                    Value = value;
                }

                public TraversableModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Not Traversable":
                            return TraversableModeOptionsEnum.NotTraversable;
                        case "Traverse Specific Types":
                            return TraversableModeOptionsEnum.TraverseSpecificTypes;
                        case "Traverse All Types":
                            return TraversableModeOptionsEnum.TraverseAllTypes;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNotTraversable()
                {
                    return Value == "Not Traversable";
                }
                public bool IsTraverseSpecificTypes()
                {
                    return Value == "Traverse Specific Types";
                }
                public bool IsTraverseAllTypes()
                {
                    return Value == "Traverse All Types";
                }
            }

            public enum TraversableModeOptionsEnum
            {
                NotTraversable,
                TraverseSpecificTypes,
                TraverseAllTypes
            }
            public class SyncMappingToOptions
            {
                public readonly string Value;

                public SyncMappingToOptions(string value)
                {
                    Value = value;
                }

                public SyncMappingToOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Disabled":
                            return SyncMappingToOptionsEnum.Disabled;
                        case "Element Value":
                            return SyncMappingToOptionsEnum.ElementValue;
                        case "Stereotype Property Value":
                            return SyncMappingToOptionsEnum.StereotypePropertyValue;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsElementValue()
                {
                    return Value == "Element Value";
                }
                public bool IsStereotypePropertyValue()
                {
                    return Value == "Stereotype Property Value";
                }
            }

            public enum SyncMappingToOptionsEnum
            {
                Disabled,
                ElementValue,
                StereotypePropertyValue
            }
        }

    }
}