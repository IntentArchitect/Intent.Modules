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
    public static class StaticContentTemplateModelStereotypeExtensions
    {
        public static TemplateSettings GetTemplateSettings(this StaticContentTemplateModel model)
        {
            var stereotype = model.GetStereotype(TemplateSettings.DefinitionId);
            return stereotype != null ? new TemplateSettings(stereotype) : null;
        }

        public static bool HasTemplateSettings(this StaticContentTemplateModel model)
        {
            return model.HasStereotype(TemplateSettings.DefinitionId);
        }

        public static bool TryGetTemplateSettings(this StaticContentTemplateModel model, out TemplateSettings stereotype)
        {
            if (!HasTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TemplateSettings(model.GetStereotype(TemplateSettings.DefinitionId));
            return true;
        }


        public class TemplateSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1fd8fd68-ff20-437c-8c00-ac77920d7ff0";

            public TemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ContentSubfolder()
            {
                return _stereotype.GetProperty<string>("Content Subfolder");
            }

            public string BinaryFileGlobbingPatterns()
            {
                return _stereotype.GetProperty<string>("Binary File Globbing Patterns");
            }

            public string Role()
            {
                return _stereotype.GetProperty<string>("Role");
            }

            public string DefaultLocation()
            {
                return _stereotype.GetProperty<string>("Default Location");
            }

            public SeverityOptions Severity()
            {
                return new SeverityOptions(_stereotype.GetProperty<string>("Severity"));
            }

            public string Classification()
            {
                return _stereotype.GetProperty<string>("Classification");
            }

            public class SeverityOptions
            {
                public readonly string Value;

                public SeverityOptions(string value)
                {
                    Value = value;
                }

                public SeverityOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "0 - None":
                            return SeverityOptionsEnum._0None;
                        case "1 - Low":
                            return SeverityOptionsEnum._1Low;
                        case "2 - Medium":
                            return SeverityOptionsEnum._2Medium;
                        case "3 - High":
                            return SeverityOptionsEnum._3High;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool Is0None()
                {
                    return Value == "0 - None";
                }
                public bool Is1Low()
                {
                    return Value == "1 - Low";
                }
                public bool Is2Medium()
                {
                    return Value == "2 - Medium";
                }
                public bool Is3High()
                {
                    return Value == "3 - High";
                }
            }

            public enum SeverityOptionsEnum
            {
                _0None,
                _1Low,
                _2Medium,
                _3High
            }

        }

    }
}