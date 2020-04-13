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
        public static DesignerModel Create(IElement element)
        {
            switch (element.SpecializationType)
            {
                case DesignerModel.SpecializationType:
                    return new DesignerModel(element);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DesignerModel GetDesigner(IElement forElement)
        {
            var designerElement = forElement.GetParentPath()
                .Single(x => x.SpecializationType == Api.DesignerModel.SpecializationType);
            return Create(designerElement);
        }
    }
}
