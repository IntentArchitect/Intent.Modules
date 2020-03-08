using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class PackageSettings
    {
        public const string SpecializationType = "Package Settings";

        public PackageSettings(IElement element)
        {
            if (element != null && element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            CreationOptions = element?.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new CreationOption(x)).ToList()
                ?? new List<CreationOption>();
            TypeOrder = element?.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new TypeOrder(x)).ToList()
                ?? new List<TypeOrder>();
        }

        public IList<CreationOption> CreationOptions { get; set; }
        public IList<TypeOrder> TypeOrder { get; set; }
    }
}