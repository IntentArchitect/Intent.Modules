using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PackageSettings
    {
        private readonly IElement _element;
        public const string SpecializationType = "Package Settings";

        public PackageSettings(IElement element)
        {
            if (element?.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;
        }

        public static PackageSettings Create(IElement element)
        {
            return element != null ? new PackageSettings(element) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public ContextMenu MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ContextMenu.SpecializationType)
            .Select(x => new ContextMenu(x))
            .SingleOrDefault();

        public PackageSettingsPersistable ToPersistable()
        {
            return new PackageSettingsPersistable
            {
                CreationOptions = this?.MenuOptions.CreationOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = this?.MenuOptions.TypeOrder.Select(x => new TypeOrderPersistable() { Type = x.Type, Order = x.Order?.ToString() }).ToList()
            };
        }

        protected bool Equals(PackageSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }


    }
}