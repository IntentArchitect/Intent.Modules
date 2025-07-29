using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.DocumentDB.Api
{
    public static class DocumentDbProviderModelStereotypeExtensions
    {
        public static QuerySupport GetQuerySupport(this DocumentDbProviderModel model)
        {
            var stereotype = model.GetStereotype(QuerySupport.DefinitionId);
            return stereotype != null ? new QuerySupport(stereotype) : null;
        }


        public static bool HasQuerySupport(this DocumentDbProviderModel model)
        {
            return model.HasStereotype(QuerySupport.DefinitionId);
        }

        public static bool TryGetQuerySupport(this DocumentDbProviderModel model, out QuerySupport stereotype)
        {
            if (!HasQuerySupport(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new QuerySupport(model.GetStereotype(QuerySupport.DefinitionId));
            return true;
        }

        public class QuerySupport
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "03385443-bbd6-46d9-87f2-6813f7833a38";

            public QuerySupport(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool OffsetEnabled()
            {
                return _stereotype.GetProperty<bool>("Offset Enabled");
            }

            public bool CursorEnabled()
            {
                return _stereotype.GetProperty<bool>("Cursor Enabled");
            }

            public CursorFilteringOptions CursorFiltering()
            {
                return new CursorFilteringOptions(_stereotype.GetProperty<string>("Cursor Filtering"));
            }

            public CursorSortingOptions CursorSorting()
            {
                return new CursorSortingOptions(_stereotype.GetProperty<string>("Cursor Sorting"));
            }

            public bool QueryableSource()
            {
                return _stereotype.GetProperty<bool>("Queryable Source");
            }

            public class CursorFilteringOptions
            {
                public readonly string Value;

                public CursorFilteringOptions(string value)
                {
                    Value = value;
                }

                public CursorFilteringOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Disabled":
                            return CursorFilteringOptionsEnum.Disabled;
                        case "Enabled (Not Nullable)":
                            return CursorFilteringOptionsEnum.EnabledNotNullable;
                        case "Enabled (Nullable)":
                            return CursorFilteringOptionsEnum.EnabledNullable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsEnabledNotNullable()
                {
                    return Value == "Enabled (Not Nullable)";
                }
                public bool IsEnabledNullable()
                {
                    return Value == "Enabled (Nullable)";
                }
            }

            public enum CursorFilteringOptionsEnum
            {
                Disabled,
                EnabledNotNullable,
                EnabledNullable
            }
            public class CursorSortingOptions
            {
                public readonly string Value;

                public CursorSortingOptions(string value)
                {
                    Value = value;
                }

                public CursorSortingOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Disabled":
                            return CursorSortingOptionsEnum.Disabled;
                        case "Enabled (Without Direction)":
                            return CursorSortingOptionsEnum.EnabledWithoutDirection;
                        case "Enabled (With Direction)":
                            return CursorSortingOptionsEnum.EnabledWithDirection;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsEnabledWithoutDirection()
                {
                    return Value == "Enabled (Without Direction)";
                }
                public bool IsEnabledWithDirection()
                {
                    return Value == "Enabled (With Direction)";
                }
            }

            public enum CursorSortingOptionsEnum
            {
                Disabled,
                EnabledWithoutDirection,
                EnabledWithDirection
            }

        }

    }
}