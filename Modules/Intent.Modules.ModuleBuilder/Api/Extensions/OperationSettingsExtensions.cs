using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class OperationSettingsExtensions
    {
        public static AdditionalProperties GetAdditionalProperties(this IOperationSettings model)
        {
            var stereotype = model.GetStereotype("Additional Properties");
            return stereotype != null ? new AdditionalProperties(stereotype) : null;
        }


        public class AdditionalProperties
        {
            private IStereotype _stereotype;

            public AdditionalProperties(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Text()
            {
                return _stereotype.GetProperty<string>("Text");
            }

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string DisplayFunction()
            {
                return _stereotype.GetProperty<string>("Display Function");
            }

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public bool AllowRename()
            {
                return _stereotype.GetProperty<bool>("Allow Rename");
            }

            public bool AllowDuplicateNames()
            {
                return _stereotype.GetProperty<bool>("Allow Duplicate Names");
            }

            public bool AllowFindinView()
            {
                return _stereotype.GetProperty<bool>("Allow Find in View");
            }

            public string DefaultTypeId()
            {
                return _stereotype.GetProperty<string>("Default Type Id");
            }

            public bool IsStereotypePropertyTarget()
            {
                return _stereotype.GetProperty<bool>("Is Stereotype Property Target");
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
            }

        }

    }
}