using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.ModuleBuilder.Api.Factories
{
    public class DesignerModelFactory
    {
        public static DesignerSettingsModel GetDesignerSettings(ICanBeReferencedType forElement)
        {
            var designerElement = ((forElement as IElement) ?? ((IAssociationEnd) forElement).Association.SourceEnd.TypeReference.Element as IElement).GetParentPath()
                .Single(x => x.SpecializationType == Api.DesignerSettingsModel.SpecializationType);
            return new DesignerSettingsModel(designerElement);
        }
    }
}
