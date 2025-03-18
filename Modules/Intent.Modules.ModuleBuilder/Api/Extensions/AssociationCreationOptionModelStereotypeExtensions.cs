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
    public static class AssociationCreationOptionModelStereotypeExtensions
    {
        public static OptionSettings GetOptionSettings(this AssociationCreationOptionModel model)
        {
            var stereotype = model.GetStereotype(OptionSettings.DefinitionId);
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }

        public static bool HasOptionSettings(this AssociationCreationOptionModel model)
        {
            return model.HasStereotype(OptionSettings.DefinitionId);
        }

        public static bool TryGetOptionSettings(this AssociationCreationOptionModel model, out OptionSettings stereotype)
        {
            if (!HasOptionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OptionSettings(model.GetStereotype(OptionSettings.DefinitionId));
            return true;
        }

        public static SourceEndDefaults GetSourceEndDefaults(this AssociationCreationOptionModel model)
        {
            var stereotype = model.GetStereotype(SourceEndDefaults.DefinitionId);
            return stereotype != null ? new SourceEndDefaults(stereotype) : null;
        }


        public static bool HasSourceEndDefaults(this AssociationCreationOptionModel model)
        {
            return model.HasStereotype(SourceEndDefaults.DefinitionId);
        }

        public static bool TryGetSourceEndDefaults(this AssociationCreationOptionModel model, out SourceEndDefaults stereotype)
        {
            if (!HasSourceEndDefaults(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SourceEndDefaults(model.GetStereotype(SourceEndDefaults.DefinitionId));
            return true;
        }

        public static TargetEndDefaults GetTargetEndDefaults(this AssociationCreationOptionModel model)
        {
            var stereotype = model.GetStereotype(TargetEndDefaults.DefinitionId);
            return stereotype != null ? new TargetEndDefaults(stereotype) : null;
        }


        public static bool HasTargetEndDefaults(this AssociationCreationOptionModel model)
        {
            return model.HasStereotype(TargetEndDefaults.DefinitionId);
        }

        public static bool TryGetTargetEndDefaults(this AssociationCreationOptionModel model, out TargetEndDefaults stereotype)
        {
            if (!HasTargetEndDefaults(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TargetEndDefaults(model.GetStereotype(TargetEndDefaults.DefinitionId));
            return true;
        }


        public class OptionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "9682fe8b-bf51-4c87-b198-5fcfe8983852";

            public OptionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string ShortcutMacOS()
            {
                return _stereotype.GetProperty<string>("Shortcut (macOS)");
            }

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public int? TypeOrder()
            {
                return _stereotype.GetProperty<int?>("Type Order");
            }

            public int? MenuGroup()
            {
                return _stereotype.GetProperty<int?>("Menu Group");
            }

            public bool AllowMultiple()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple");
            }

            public string IsOptionVisibleFunction()
            {
                return _stereotype.GetProperty<string>("Is Option Visible Function");
            }

            public bool BottomDivider()
            {
                return _stereotype.GetProperty<bool>("Bottom Divider");
            }

            public bool TopDivider()
            {
                return _stereotype.GetProperty<bool>("Top Divider");
            }

        }

        public class SourceEndDefaults
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0238ed04-0e7b-42e8-9674-f7d1fd67275b";

            public SourceEndDefaults(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsNavigable()
            {
                return _stereotype.GetProperty<bool>("Is Navigable");
            }

            public bool IsNullable()
            {
                return _stereotype.GetProperty<bool>("Is Nullable");
            }

            public bool IsCollection()
            {
                return _stereotype.GetProperty<bool>("Is Collection");
            }

        }

        public class TargetEndDefaults
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "99511817-15b6-462a-9c3c-449da1b49451";

            public TargetEndDefaults(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsNavigable()
            {
                return _stereotype.GetProperty<bool>("Is Navigable");
            }

            public bool IsNullable()
            {
                return _stereotype.GetProperty<bool>("Is Nullable");
            }

            public bool IsCollection()
            {
                return _stereotype.GetProperty<bool>("Is Collection");
            }

        }

    }
}