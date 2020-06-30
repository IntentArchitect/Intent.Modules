using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class DesignerModelExtensions
    {
        public static DesignerConfig GetDesignerConfig(this DesignerModel model)
        {
            var stereotype = model.GetStereotype("Designer Config");
            return stereotype != null ? new DesignerConfig(stereotype) : null;
        }

        public static bool HasDesignerConfig(this DesignerModel model)
        {
            return model.HasStereotype("Designer Config");
        }


        public class DesignerConfig
        {
            private IStereotype _stereotype;

            public DesignerConfig(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public int? DisplayOrder()
            {
                return _stereotype.GetProperty<int?>("Display Order");
            }

        }

    }
}