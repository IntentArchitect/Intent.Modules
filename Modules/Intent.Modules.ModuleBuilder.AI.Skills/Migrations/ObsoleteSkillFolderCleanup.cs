using System.Diagnostics;
using System.Linq;
using Intent.Persistence.V2;

namespace Intent.Modules.ModuleBuilder.AI.Skills.Migrations
{
    /// <summary>
    /// Removes the Folder elements this module implied under the AI.Context.Skills anchor back when a
    /// File Template's sub-path was expressed via "Default Location" instead of the ctor's
    /// relativeLocation. Safe to call from more than one Version Migration: a consumer's transition away
    /// from those folders can settle in a later Software Factory pass than the one that ran the first
    /// migration to see it, so a later version's migration may be what actually finds them empty.
    /// </summary>
    internal static class ObsoleteSkillFolderCleanup
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string FolderTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";

        private static readonly string[] ObsoleteSkillFolderNames =
            [
            "add-association-type",
            "add-designer-extension",
            "architecture-templates",
            "file-builder-expert",
            "intent-domain-interactions-expert",
            "intent-mapping-architect",
            "intent-metadata-consumer",
            "intent-module-orchestrator",
            "module-building-strategies",
            "module-debugging",
            "module-docs",
            "module-svg-icon",
            "module-versioning",
            ];

        public static void Run(IPersistenceLoader persistenceLoader)
        {
            var application = persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigners().SingleOrDefault(d => d.Id == CodebaseStructureDesignerId);
            var package = designer?.GetPackages(false, false).FirstOrDefault();
            var skillsAnchor = package?.FindChildElements(x => x.Name == "AI.Context.Skills" && x.SpecializationType == "Output Anchor").SingleOrDefault();

            if (package == null || skillsAnchor == null)
            {
                return;
            }

            var removedAny = false;

            foreach (var skillName in ObsoleteSkillFolderNames)
            {
                // 1. Identify the folder which used to house the template.
                var folder = package.Classes.FirstOrDefault(x =>
                    x.SpecializationTypeId == FolderTypeId &&
                    x.Name == skillName &&
                    x.ParentFolderId == skillsAnchor.ParentFolderId);

                if (folder == null)
                {
                    continue;
                }

                // 2. Remove its nested folders.
                RemoveNestedFolders(package, folder);

                // 3. Remove the folder itself.
                package.Classes.Remove(folder);
                removedAny = true;
            }

            if (removedAny)
            {
                package.Save();
            }
        }

        private static void RemoveNestedFolders(IPackageModelPersistable package, IElementPersistable folder)
        {
            foreach (var child in folder.ChildElements.Where(x => x.SpecializationTypeId == FolderTypeId).ToList())
            {
                RemoveNestedFolders(package, child);
                package.Classes.Remove(child);
            }
        }
    }
}
