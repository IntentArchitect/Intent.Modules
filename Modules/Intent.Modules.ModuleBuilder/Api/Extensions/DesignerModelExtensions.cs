using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
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

        public static OutputConfiguration GetOutputConfiguration(this DesignerModel model)
        {
            var stereotype = model.GetStereotype("Output Configuration");
            return stereotype != null ? new OutputConfiguration(stereotype) : null;
        }

        public static bool HasOutputConfiguration(this DesignerModel model)
        {
            return model.HasStereotype("Output Configuration");
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