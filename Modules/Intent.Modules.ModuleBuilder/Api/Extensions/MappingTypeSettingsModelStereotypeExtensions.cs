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
    public static class MappingTypeSettingsModelStereotypeExtensions
    {
        public static MappingTypeSettings GetMappingTypeSettings(this MappingTypeSettingsModel model)
        {
            var stereotype = model.GetStereotype(MappingTypeSettings.DefinitionId);
            return stereotype != null ? new MappingTypeSettings(stereotype) : null;
        }


        public static bool HasMappingTypeSettings(this MappingTypeSettingsModel model)
        {
            return model.HasStereotype(MappingTypeSettings.DefinitionId);
        }

        public static bool TryGetMappingTypeSettings(this MappingTypeSettingsModel model, out MappingTypeSettings stereotype)
        {
            if (!HasMappingTypeSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingTypeSettings(model.GetStereotype(MappingTypeSettings.DefinitionId));
            return true;
        }

        public class MappingTypeSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5112db0d-9496-4a00-aff2-50c438050da6";

            public MappingTypeSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SourcesOptions Sources()
            {
                return new SourcesOptions(_stereotype.GetProperty<string>("Sources"));
            }

            public IElement[] SourceTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Source Types") ?? new IElement[0];
            }

            public string SourceTypesFilter()
            {
                return _stereotype.GetProperty<string>("Source Types Filter");
            }

            public SourceArrowTypeOptions SourceArrowType()
            {
                return new SourceArrowTypeOptions(_stereotype.GetProperty<string>("Source Arrow Type"));
            }

            public TargetsOptions Targets()
            {
                return new TargetsOptions(_stereotype.GetProperty<string>("Targets"));
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types") ?? new IElement[0];
            }

            public string TargetTypesFilter()
            {
                return _stereotype.GetProperty<string>("Target Types Filter");
            }

            public TargetArrowTypeOptions TargetArrowType()
            {
                return new TargetArrowTypeOptions(_stereotype.GetProperty<string>("Target Arrow Type"));
            }

            public RepresentsOptions Represents()
            {
                return new RepresentsOptions(_stereotype.GetProperty<string>("Represents"));
            }

            public IElement[] SyncSourceChildTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Sync Source Child Types") ?? new IElement[0];
            }

            public string LineColor()
            {
                return _stereotype.GetProperty<string>("Line Color");
            }

            public string LineDashArray()
            {
                return _stereotype.GetProperty<string>("Line Dash Array");
            }

            public string ValidationFunction()
            {
                return _stereotype.GetProperty<string>("Validation Function");
            }

            public class SourcesOptions
            {
                public readonly string Value;

                public SourcesOptions(string value)
                {
                    Value = value;
                }

                public SourcesOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Data Types":
                            return SourcesOptionsEnum.DataTypes;
                        case "Invokable Types":
                            return SourcesOptionsEnum.InvokableTypes;
                        case "Event Types":
                            return SourcesOptionsEnum.EventTypes;
                        case "Specific Types":
                            return SourcesOptionsEnum.SpecificTypes;
                        case "All Types":
                            return SourcesOptionsEnum.AllTypes;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDataTypes()
                {
                    return Value == "Data Types";
                }
                public bool IsInvokableTypes()
                {
                    return Value == "Invokable Types";
                }
                public bool IsEventTypes()
                {
                    return Value == "Event Types";
                }
                public bool IsSpecificTypes()
                {
                    return Value == "Specific Types";
                }
                public bool IsAllTypes()
                {
                    return Value == "All Types";
                }
            }

            public enum SourcesOptionsEnum
            {
                DataTypes,
                InvokableTypes,
                EventTypes,
                SpecificTypes,
                AllTypes
            }
            public class SourceArrowTypeOptions
            {
                public readonly string Value;

                public SourceArrowTypeOptions(string value)
                {
                    Value = value;
                }

                public SourceArrowTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "None":
                            return SourceArrowTypeOptionsEnum.None;
                        case "Solid Arrow":
                            return SourceArrowTypeOptionsEnum.SolidArrow;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsSolidArrow()
                {
                    return Value == "Solid Arrow";
                }
            }

            public enum SourceArrowTypeOptionsEnum
            {
                None,
                SolidArrow
            }
            public class TargetsOptions
            {
                public readonly string Value;

                public TargetsOptions(string value)
                {
                    Value = value;
                }

                public TargetsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Data Types":
                            return TargetsOptionsEnum.DataTypes;
                        case "Invokable Types":
                            return TargetsOptionsEnum.InvokableTypes;
                        case "Event Types":
                            return TargetsOptionsEnum.EventTypes;
                        case "Specific Types":
                            return TargetsOptionsEnum.SpecificTypes;
                        case "All Types":
                            return TargetsOptionsEnum.AllTypes;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDataTypes()
                {
                    return Value == "Data Types";
                }
                public bool IsInvokableTypes()
                {
                    return Value == "Invokable Types";
                }
                public bool IsEventTypes()
                {
                    return Value == "Event Types";
                }
                public bool IsSpecificTypes()
                {
                    return Value == "Specific Types";
                }
                public bool IsAllTypes()
                {
                    return Value == "All Types";
                }
            }

            public enum TargetsOptionsEnum
            {
                DataTypes,
                InvokableTypes,
                EventTypes,
                SpecificTypes,
                AllTypes
            }
            public class TargetArrowTypeOptions
            {
                public readonly string Value;

                public TargetArrowTypeOptions(string value)
                {
                    Value = value;
                }

                public TargetArrowTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "None":
                            return TargetArrowTypeOptionsEnum.None;
                        case "Solid Arrow":
                            return TargetArrowTypeOptionsEnum.SolidArrow;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsSolidArrow()
                {
                    return Value == "Solid Arrow";
                }
            }

            public enum TargetArrowTypeOptionsEnum
            {
                None,
                SolidArrow
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
            }

            public enum RepresentsOptionsEnum
            {
                Data,
                Invokable
            }
        }

    }
}