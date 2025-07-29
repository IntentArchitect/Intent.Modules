#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.RDBMS.Migrations
{
    public class Migration_03_07_06_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_03_07_06_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.RDBMS";
        [IntentFully]
        public string ModuleVersion => "3.7.6-pre.0";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            foreach (var package in application.GetDesigners().SelectMany(x => x.GetPackages()))
            {
                bool requiresSave = false;

                var attributes = package.GetElementsOfType(AttributeModel.SpecializationTypeId);
                foreach (var attribute in attributes)
                {
                    if (!attribute.Stereotypes.TryGet(AttributeModelStereotypeExtensions.Column.DefinitionId, out var stereotype))
                    {
                        continue;
                    }

                    requiresSave = true;

                    stereotype.Properties.AddOrUpdate("fae5d195-d9f0-498e-938b-922362d8c37c", "Collation", property =>
                    {
                        property.IsActive = true;
                    });
                }

                if (requiresSave)
                {
                    package.Save();
                }
            }
        }

        public void Down()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            foreach (var package in application.GetDesigners().SelectMany(x => x.GetPackages()))
            {
                bool requiresSave = false;

                var attributes = package.GetElementsOfType(AttributeModel.SpecializationTypeId);
                foreach (var attribute in attributes)
                {
                    if (!attribute.Stereotypes.TryGet(AttributeModelStereotypeExtensions.TextConstraints.DefinitionId, out var stereotype))
                    {
                        continue;
                    }

                    requiresSave = true;

                    if (stereotype.Properties.TryGet("fae5d195-d9f0-498e-938b-922362d8c37c", out var collationProperty))
                    {
                        stereotype.Properties.Remove(collationProperty);
                    }
                }

                if (requiresSave)
                {
                    package.Save();
                }
            }
        }
    }

    internal static class ExtensionMethods
    {
        public static IStereotypePropertyPersistable AddOrUpdate(this IStereotypePropertyPersistableCollection properties, string definitionId, string name, Action<IStereotypePropertyPersistable> configure)
        {
            var property = properties.SingleOrDefault(x => x.DefinitionId == definitionId) ??
                           properties.Add(definitionId, name, string.Empty);

            configure(property);

            return property;
        }
    }
}