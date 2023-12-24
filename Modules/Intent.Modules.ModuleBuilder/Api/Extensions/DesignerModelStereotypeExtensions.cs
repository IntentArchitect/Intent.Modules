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
    public static class DesignerModelStereotypeExtensions
    {
        public static DesignerConfig GetDesignerConfig(this DesignerModel model)
        {
            var stereotype = model.GetStereotype("6761be76-7c09-4ad4-8a65-82c6d17992db");
            return stereotype != null ? new DesignerConfig(stereotype) : null;
        }

        public static bool HasDesignerConfig(this DesignerModel model)
        {
            return model.HasStereotype("6761be76-7c09-4ad4-8a65-82c6d17992db");
        }

        public static bool TryGetDesignerConfig(this DesignerModel model, out DesignerConfig stereotype)
        {
            if (!HasDesignerConfig(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DesignerConfig(model.GetStereotype("6761be76-7c09-4ad4-8a65-82c6d17992db"));
            return true;
        }

        public static OutputConfiguration GetOutputConfiguration(this DesignerModel model)
        {
            var stereotype = model.GetStereotype("82a8b98c-0306-4309-b564-146668de0e74");
            return stereotype != null ? new OutputConfiguration(stereotype) : null;
        }

        public static bool HasOutputConfiguration(this DesignerModel model)
        {
            return model.HasStereotype("82a8b98c-0306-4309-b564-146668de0e74");
        }

        public static bool TryGetOutputConfiguration(this DesignerModel model, out OutputConfiguration stereotype)
        {
            if (!HasOutputConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OutputConfiguration(model.GetStereotype("82a8b98c-0306-4309-b564-146668de0e74"));
            return true;
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

        public class OutputConfiguration
        {
            private IStereotype _stereotype;

            public OutputConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement PackageType()
            {
                return _stereotype.GetProperty<IElement>("Package Type");
            }

            public IElement RoleType()
            {
                return _stereotype.GetProperty<IElement>("Role Type");
            }

            public IElement TemplateOutputType()
            {
                return _stereotype.GetProperty<IElement>("Template Output Type");
            }

            public IElement FolderType()
            {
                return _stereotype.GetProperty<IElement>("Folder Type");
            }

        }

    }
}