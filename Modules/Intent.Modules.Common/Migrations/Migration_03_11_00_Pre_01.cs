using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Common.Migrations
{
    public class Migration_03_11_00_Pre_01 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string AIContextAnchorId = "5489f14c-82ab-438a-9fef-67604cb25122";

        public Migration_03_11_00_Pre_01(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common";
        [IntentFully]
        public string ModuleVersion => "3.11.0-pre.1";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(CodebaseStructureDesignerId);
            var package = designer.GetPackages().FirstOrDefault();

            if (package != null)
            {
                if (!package.Classes.TryGet(AIContextAnchorId, out var _))
                {

                    var agentsFolder = package.AddFolder(package.Id, ".agents");
                    var instructionsFolder = package.AddFolder(agentsFolder.Id, "instructions");
                    var skillsFolder = package.AddFolder(agentsFolder.Id, "skills");

                    package.AddAnchor(agentsFolder.Id, "AI.Context", AIContextAnchorId);
                    package.AddAnchor(instructionsFolder.Id, "AI.Context.Instructions");
                    package.AddAnchor(skillsFolder.Id, "AI.Context.SKills");


                    package.Save();
                }
            }
        }


        public void Down()
        {
        }
    }

    internal static class PackageExtensions
    {
        private const string NonVSFolderSpecicalizationTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";
        private const string AnchorSpecicalizationTypeId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        internal static IElementPersistable AddAnchor(this IPackageModelPersistable item, string parentId, string name, string anchorId = null)
        {
            return item.Classes.Add(
                        id: anchorId ?? Guid.NewGuid().ToString(),
                        specializationType: "Output Anchor",
                        specializationTypeId: AnchorSpecicalizationTypeId,
                        name: name,
                        parentId: parentId);
        }

        internal static IElementPersistable AddFolder(this IPackageModelPersistable item, string parentId, string name)
        {
            return item.Classes.Add(
                        id: Guid.NewGuid().ToString(),
                        specializationType: "Folder",
                        specializationTypeId: NonVSFolderSpecicalizationTypeId,
                        name: name,
                        parentId: parentId);
        }
    }
}