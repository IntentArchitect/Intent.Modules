using System.Linq;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Repositories.Migrations
{
    public class Migration_03_06_00_Alpha_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_03_06_00_Alpha_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.Domain.Repositories";
        [IntentFully]
        public string ModuleVersion => "3.6.0-alpha.0";

        public void Up()
        {
            var domainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
            var domainPackageTypeId = "1a824508-4623-45d9-accc-f572091ade5a";
            var domainPackages = _persistenceLoader.LoadCurrentApplication().GetDesigner(domainDesignerId).GetPackages()
                .Where(x => x.SpecializationTypeId == domainPackageTypeId)
                .ToList();

            foreach (var package in domainPackages)
            {
                foreach (var repository in package.GetElementsOfType("96ffceb2-a70a-4b69-869b-0df436c470c3")) // Repository
                {
                    foreach (var child in repository.ChildElements)
                    {
                        if (!child.Traits.Exists("0a932465-ea1f-42e7-b1eb-9f90aa8961fd"))
                        {
                            child.Traits.Add("0a932465-ea1f-42e7-b1eb-9f90aa8961fd", "[Invokable]");
                        }
                    }
                }
                package.Save();
            }
        }

        public void Down()
        {
        }
    }
}