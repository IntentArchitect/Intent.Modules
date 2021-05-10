using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class TemplateRegistrationModel : IHasFolder, IHasStereotypes, IMetadataModel, IHasName
    {
        public const string SpecializationType = "Template Registration";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public TemplateRegistrationModel(IElement element, string requiredType = SpecializationType)
        {
            if (element.TypeReference == null)
            {
                throw new Exception("Cannot create 'TemplateRegistration'. Element must have a type reference.");
            }
            if (!SpecializationType.Equals(element.TypeReference?.Element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'TemplateRegistration' from element that has type-reference with specialization type '{element.TypeReference?.Element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = element.ParentElement != null ? new FolderModel(element.ParentElement) : null;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;


        public DesignerSettingsModel GetDesignerSettings()
        {
            return GetModelType()?.DesignerSettings;
        }

        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel GetModule()
        {
            return new IntentModuleModel(_element.Package);
        }

        public DesignerModel GetDesigner()
        {
            return this.GetTemplateSettings()?.Designer();
        }

        public ModelerModelType GetModelType()
        {
            return this.GetTemplateSettings()?.ModelType() != null ? new ModelerModelType(this.GetTemplateSettings().ModelType()) : null;
        }

        public string GetModelName()
        {
            if (this.GetTemplateSettings().Source().IsLookupType())
            {
                var modelType = this.GetModelType();
                if (this.IsSingleFileTemplateRegistration())
                {
                    return modelType == null ? "object" : $"IList<{modelType.FullyQualifiedName}>";
                }
                return modelType?.FullyQualifiedName ?? "object";
            }

            if (this.GetTemplateSettings().Source().IsCustomType())
            {
                var modelType = this.GetTemplateSettings().ModelName() ?? "object";
                if (this.IsSingleFileTemplateRegistration())
                {
                    return $"IList<{modelType}>";
                }
                return modelType;
            }
            throw new Exception("Could not determine model type for template [" + this.ToString() + "]");
        }

        public string GetRole()
        {
            return this.GetTemplateSettings().GetRole();
        }

        public string GetLocation()
        {
            return this.GetTemplateSettings().GetDefaultLocation();
        }

        public FolderModel Folder { get; }

        public bool IsSingleFileTemplateRegistration()
        {
            return _element.ReferencesSingleFile();
        }

        public bool IsFilePerModelTemplateRegistration()
        {
            return _element.ReferencesFilePerModel();
        }

        public bool IsCustomTemplateRegistration()
        {
            return _element.ReferencesCustom();
        }


        [IntentManaged(Mode.Fully)]
        public bool Equals(TemplateRegistrationModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TemplateRegistrationModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);
        public const string SpecializationTypeId = "502551f7-c2d7-4d65-ad9f-13bd1e9e96d5";

        public string Comment => _element.Comment;
    }
}