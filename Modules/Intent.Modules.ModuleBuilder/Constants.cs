namespace Intent.ModuleBuilder
{
    public static class Constants
    {
        public static class ElementSpecializationTypes
        {
            public const string Modeler = "Modeler";
            public const string PackageSettings = "Package Settings";
            public const string CreationOptions = "Creation Options";
            public const string ModelerFolder = "Modeler Folder";
            public const string ElementSettings = "Element Settings";
            public const string AttributeSetting = "Attribute Setting";
            public const string AttributeSettings = "Attribute Settings";
            public const string AssociationEndSettings = "Association End Settings";
            public const string AssociationSettings = "Association Settings";
            public const string DestinationEnd = "Destination End";
            public const string SourceEnd = "Source End";
            public const string LiteralSettings = "Literal Settings";
        }

        public static class AttributeSpecializationTypes
        {
            public const string CreationOption = "Creation Option";
        }

        public static class Stereotypes
        {
            public static class IconFull
            {
                public const string Name = "Icon (Full)";

                public static class Property
                {
                    public const string Type = "Type";
                    public const string Source = "Source";
                }
            }

            public static class IconFullExpanded
            {
                public const string Name = "Icon (Full, Expanded)";

                public static class Property
                {
                    public const string Type = "Type";
                    public const string Source = "Source";
                }
            }

            public static class DefaultCreationOptions
            {
                public const string Name = "Default Creation Options";

                public static class Property
                {
                    public const string Text = "Text";
                    public const string Shortcut = "Shortcut";
                    public const string DefaultName = "Default Name";
                    public const string TypeOrder = "Type Order";
                }
            }

            public static class AttributeAdditionalProperties
            {
                public const string Name = "Additional Properties";

                public static class Property
                {
                    public const string Text = "Text";
                    public const string Shortcut = "Shortcut";
                    public const string DisplayFunction = "Display Function";
                    public const string DefaultName = "Default Name";
                    public const string AllowRename = "Allow Rename";
                    public const string AllowDuplicateNames = "Allow Duplicate Names";
                    public const string AllowFindInView = "Allow Find in View";
                    public const string DefaultTypeId = "Default Type Id";
                    public const string IsStereotypePropertyTarget = "Is Stereotype Property Target";
                }
            }

            public static class ElementAdditionalProperties
            {
                public const string Name = "Additional Properties";

                public static class Property
                {
                    public const string AllowRename = "Allow Rename";
                    public const string AllowAbstract = "Allow Abstract";
                    public const string AllowGenericTypes = "Allow Generic Types";
                    public const string AllowMapping = "Allow Mapping";
                    public const string AllowSorting = "Allow Sorting";
                    public const string AllowFindInView = "Allow Find in View";
                    public const string IsStereotypePropertyTarget = "Is Stereotype Property Target";
                }
            }

            public static class LiteralSettingsAdditionalProperties
            {
                public const string Name = "Additional Properties";

                public static class Property
                {
                    public const string Text = "Text";
                    public const string Shortcut = "Shortcut";
                    public const string DefaultName = "Default Name";
                    public const string AllowRename = "Allow Rename";
                    public const string AllowDuplicateNames = "Allow Duplicate Names";
                    public const string AllowFindInView = "Allow Find in View";
                }
            }

            public static class AssociationEndSettingsAdditionalProperties
            {
                public const string Name = "Additional Properties";

                public static class Property
                {
                    public const string IsNavigableEnabled = "Is Navigable Enabled";
                    public const string IsNullableEnabled = "Is Nullable Enabled";
                    public const string IsCollectionEnabled = "Is Collection Enabled";
                    public const string IsNavigableDefault = "Is Navigable Default";
                    public const string IsNullableDefault = "Is Nullable Default";
                    public const string IsCollectionDefault = "Is Collection Default";
                }
            }
        }
    }
}
