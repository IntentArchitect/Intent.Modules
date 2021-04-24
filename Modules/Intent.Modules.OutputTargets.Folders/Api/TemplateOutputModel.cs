using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    [IntentManaged(Mode.Merge)]
    public class TemplateOutputModel : IHasStereotypes, IMetadataModel, IOutputTargetTemplate, IHasName, IHasFolder
    {
        public const string SpecializationType = "Template Output";
        public const string SpecializationTypeId = "09de2192-2507-41a2-8044-286c7ecadec2";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public TemplateOutputModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        [IntentManaged(Mode.Ignore)]
        string IOutputTargetTemplate.Id => _element.Name;

        [IntentManaged(Mode.Ignore)]
        public IEnumerable<string> RequiredFrameworks => new string[0];

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(TemplateOutputModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TemplateOutputModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;

        public FolderModel Folder { get; }
    }
}