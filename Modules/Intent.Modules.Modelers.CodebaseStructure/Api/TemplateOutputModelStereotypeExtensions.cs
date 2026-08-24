using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    public static class TemplateOutputModelStereotypeExtensions
    {
        public static OutputClassification GetOutputClassification(this TemplateOutputModel model)
        {
            var stereotype = model.GetStereotype(OutputClassification.DefinitionId);
            return stereotype != null ? new OutputClassification(stereotype) : null;
        }


        public static bool HasOutputClassification(this TemplateOutputModel model)
        {
            return model.HasStereotype(OutputClassification.DefinitionId);
        }

        public static bool TryGetOutputClassification(this TemplateOutputModel model, out OutputClassification stereotype)
        {
            if (!HasOutputClassification(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OutputClassification(model.GetStereotype(OutputClassification.DefinitionId));
            return true;
        }
        public static TemplateOutputSettings GetTemplateOutputSettings(this TemplateOutputModel model)
        {
            var stereotype = model.GetStereotype(TemplateOutputSettings.DefinitionId);
            return stereotype != null ? new TemplateOutputSettings(stereotype) : null;
        }


        public static bool HasTemplateOutputSettings(this TemplateOutputModel model)
        {
            return model.HasStereotype(TemplateOutputSettings.DefinitionId);
        }

        public static bool TryGetTemplateOutputSettings(this TemplateOutputModel model, out TemplateOutputSettings stereotype)
        {
            if (!HasTemplateOutputSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TemplateOutputSettings(model.GetStereotype(TemplateOutputSettings.DefinitionId));
            return true;
        }

        public class OutputClassification
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "98cff892-b03e-4d9c-b1dc-031cb0c900ee";

            public OutputClassification(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] Classification()
            {
                return _stereotype.GetProperty<IElement[]>("Classification") ?? new IElement[0];
            }

            public SeverityOptions Severity()
            {
                return new SeverityOptions(_stereotype.GetProperty<string>("Severity"));
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

        public class TemplateOutputSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "967ae6f6-cc06-4b67-a391-44ed1aac1959";

            public TemplateOutputSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Enabled");
            }

            public string RegistrationFilter()
            {
                return _stereotype.GetProperty<string>("Registration Filter");
            }

        }

    }
}