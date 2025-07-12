using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ModuleMigrationsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Module Migrations";
        public const string SpecializationTypeId = "8cfd2eab-c1bf-46bc-a86f-3ed0263a380f";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ModuleMigrationsModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public OnInstallMigrationModel OnInstallMigration => _element.ChildElements
            .GetElementsOfType(OnInstallMigrationModel.SpecializationTypeId)
            .Select(x => new OnInstallMigrationModel(x))
            .SingleOrDefault();

        public IList<VersionMigrationModel> VersionMigrations => _element.ChildElements
            .GetElementsOfType(VersionMigrationModel.SpecializationTypeId)
            .Select(x => new VersionMigrationModel(x))
            .ToList();

        public OnUninstallMigrationModel OnUninstallMigration => _element.ChildElements
            .GetElementsOfType(OnUninstallMigrationModel.SpecializationTypeId)
            .Select(x => new OnUninstallMigrationModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ModuleMigrationsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleMigrationsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ModuleMigrationsModelExtensions
    {

        public static bool IsModuleMigrationsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModuleMigrationsModel.SpecializationTypeId;
        }

        public static ModuleMigrationsModel AsModuleMigrationsModel(this ICanBeReferencedType type)
        {
            return type.IsModuleMigrationsModel() ? new ModuleMigrationsModel((IElement)type) : null;
        }
    }
}