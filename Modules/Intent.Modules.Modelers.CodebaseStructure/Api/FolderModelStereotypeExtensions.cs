using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static CustomFileClassification GetCustomFileClassification(this FolderModel model)
        {
            var stereotype = model.GetStereotype(CustomFileClassification.DefinitionId);
            return stereotype != null ? new CustomFileClassification(stereotype) : null;
        }

        public static IReadOnlyCollection<CustomFileClassification> GetCustomFileClassifications(this FolderModel model)
        {
            var stereotypes = model
                .GetStereotypes(CustomFileClassification.DefinitionId)
                .Select(stereotype => new CustomFileClassification(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasCustomFileClassification(this FolderModel model)
        {
            return model.HasStereotype(CustomFileClassification.DefinitionId);
        }

        public static bool TryGetCustomFileClassification(this FolderModel model, out CustomFileClassification stereotype)
        {
            if (!HasCustomFileClassification(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CustomFileClassification(model.GetStereotype(CustomFileClassification.DefinitionId));
            return true;
        }

        public class CustomFileClassification
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1a5d919b-7ca2-4f2f-8a0d-0995aa6b02ac";

            public CustomFileClassification(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Classification()
            {
                return _stereotype.GetProperty<string>("Classification");
            }

            public SeverityOptions Severity()
            {
                return new SeverityOptions(_stereotype.GetProperty<string>("Severity"));
            }

            public string Glob()
            {
                return _stereotype.GetProperty<string>("Glob");
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
                        case "low":
                            return SeverityOptionsEnum.Low;
                        case "medium":
                            return SeverityOptionsEnum.Medium;
                        case "high":
                            return SeverityOptionsEnum.High;
                        case "critical":
                            return SeverityOptionsEnum.Critical;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsLow()
                {
                    return Value == "low";
                }
                public bool IsMedium()
                {
                    return Value == "medium";
                }
                public bool IsHigh()
                {
                    return Value == "high";
                }
                public bool IsCritical()
                {
                    return Value == "critical";
                }
            }

            public enum SeverityOptionsEnum
            {
                Low,
                Medium,
                High,
                Critical
            }
        }

    }
}