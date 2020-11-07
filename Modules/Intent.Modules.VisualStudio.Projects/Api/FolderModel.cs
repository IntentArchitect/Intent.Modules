using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FolderModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Folder";
        public const string SpecializationTypeId = "3407a825-1331-4f3f-89a4-901903ed97ce";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public FolderModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = element.ParentElement?.SpecializationType == FolderModel.SpecializationType ? new FolderModel(element.ParentElement) : null;
        }

        public FolderModel Folder { get; }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public IList<FolderModel> GetFolderPath()
        {
            List<FolderModel> result = new List<FolderModel>();

            var current = Folder;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.Folder;
            }
            return result;
        }



        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(FolderModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FolderModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<RoleModel> Roles => _element.ChildElements
            .Where(x => x.SpecializationType == RoleModel.SpecializationType)
            .Select(x => new RoleModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new FolderOutputTarget(this);
        }

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
                    .Where(x => x.SpecializationType == TemplateOutputModel.SpecializationType)
                    .Select(x => new TemplateOutputModel(x))
                    .ToList();
    }
}