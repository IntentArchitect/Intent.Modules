using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api.Factories
{
    public class DesignerModelFactory
    {
        public static DesignerSettingsModel Create(IElement element)
        {
            switch (element.SpecializationType)
            {
                case DesignerSettingsModel.SpecializationType:
                    return new DesignerSettingsModel(element);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DesignerSettingsModel GetDesigner(IElement forElement)
        {
            var designerElement = forElement.GetParentPath()
                .Single(x => x.SpecializationType == Api.DesignerSettingsModel.SpecializationType || x.SpecializationType == Api.DesignerExtensionModel.SpecializationType);
            return Create(designerElement);
        }
    }
}
