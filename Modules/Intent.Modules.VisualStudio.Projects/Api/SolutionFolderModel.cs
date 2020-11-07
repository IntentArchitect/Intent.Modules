using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SolutionFolderModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Solution Folder";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public SolutionFolderModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IList<SolutionFolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == SolutionFolderModel.SpecializationType)
            .Select(x => new SolutionFolderModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ClassLibraryNETCoreModel> ClassLibraryNETCores => _element.ChildElements
            .Where(x => x.SpecializationType == ClassLibraryNETCoreModel.SpecializationType)
            .Select(x => new ClassLibraryNETCoreModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ClassLibraryNETFrameworkModel> ClassLibraryNETFrameworks => _element.ChildElements
            .Where(x => x.SpecializationType == ClassLibraryNETFrameworkModel.SpecializationType)
            .Select(x => new ClassLibraryNETFrameworkModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ASPNETCoreWebApplicationModel> ASPNETCoreWebApplications => _element.ChildElements
            .Where(x => x.SpecializationType == ASPNETCoreWebApplicationModel.SpecializationType)
            .Select(x => new ASPNETCoreWebApplicationModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ASPNETWebApplicationNETFrameworkModel> ASPNETWebApplicationNETFrameworks => _element.ChildElements
            .Where(x => x.SpecializationType == ASPNETWebApplicationNETFrameworkModel.SpecializationType)
            .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<WCFServiceApplicationModel> WCFServiceApplications => _element.ChildElements
            .Where(x => x.SpecializationType == WCFServiceApplicationModel.SpecializationType)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(SolutionFolderModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolutionFolderModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "0dc2b846-c968-49eb-99b7-8776919313a8";
    }
}