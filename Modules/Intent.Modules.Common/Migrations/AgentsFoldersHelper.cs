using Intent.Persistence.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.Migrations
{
    internal class AgentsFoldersHelper
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string AIContextAnchorId = "5489f14c-82ab-438a-9fef-67604cb25122";
        private const string AIContextRoleId = "23713fb0-c0ee-4159-bcab-b9d6de1872fc";         
        private const string FolderStructureDesignerId = "27916ede-58c6-4356-87b1-0b0628d1e2a7";

        internal static void AddAgentsFolder(IApplicationPersistable application)
        {
            // first check codebase structure, then check for folder structure designer
            IApplicationDesignerPersistable designer = null;
            if (application.GetDesigners().Any(d => d.Id == CodebaseStructureDesignerId))
            {
                designer = application.TryGetDesigner(CodebaseStructureDesignerId);
            }
            if (application.GetDesigners().Any(d => d.Id == FolderStructureDesignerId))
            {
                designer = application.TryGetDesigner(FolderStructureDesignerId);
            }
            var package = designer?.GetPackages().FirstOrDefault();

            if (package != null && designer.Id == CodebaseStructureDesignerId)
            {
                if (!package.Classes.TryGet(AIContextAnchorId, out var _) &&
                    !package.Classes.Any(c => c.Name == "AI.Context" && c.SpecializationTypeId == PackageExtensions.AnchorSpecicalizationTypeId))
                {
                    var agentsFolder = package.AddFolder(package.Id, ".agents");
                    var instructionsFolder = package.AddFolder(agentsFolder.Id, "instructions");
                    var skillsFolder = package.AddFolder(agentsFolder.Id, "skills");

                    package.AddAnchor(agentsFolder.Id, "AI.Context", AIContextAnchorId);
                    package.AddAnchor(instructionsFolder.Id, "AI.Context.Instructions");
                    package.AddAnchor(skillsFolder.Id, "AI.Context.Skills");

                    package.Save();
                }
            }

            if (package != null && designer.Id == FolderStructureDesignerId)
            {
                if (!package.Classes.TryGet(AIContextRoleId, out var _) &&
                    !package.Classes.Any(c => c.Name == "AI.Context" && c.SpecializationTypeId == PackageExtensions.RoleSpecializationTypeId))
                {
                    var agentsFolder = package.AddFolder(package.Id, ".agents");
                    var instructionsFolder = package.AddFolder(agentsFolder.Id, "instructions");
                    var skillsFolder = package.AddFolder(agentsFolder.Id, "skills");

                    package.AddRole(agentsFolder.Id, "AI.Context", AIContextRoleId);
                    package.AddRole(instructionsFolder.Id, "AI.Context.Instructions");
                    package.AddRole(skillsFolder.Id, "AI.Context.Skills");

                    package.Save();
                }
            }
        }
    }

    internal static class PackageExtensions
    {
        private const string NonVSFolderSpecicalizationTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";
        public const string AnchorSpecicalizationTypeId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        public const string RoleSpecializationTypeId = "8663c9f9-2852-45e7-aaa1-0883a2e6f1da";

        internal static IElementPersistable AddAnchor(this IPackageModelPersistable item, string parentId, string name, string anchorId = null)
        {
            return item.Classes.Add(
                        id: anchorId ?? Guid.NewGuid().ToString(),
                        specializationType: "Output Anchor",
                        specializationTypeId: AnchorSpecicalizationTypeId,
                        name: name,
                        parentId: parentId);
        }

        internal static IElementPersistable AddRole(this IPackageModelPersistable item, string parentId, string name, string roleId = null)
        {
            return item.Classes.Add(
                        id: roleId ?? Guid.NewGuid().ToString(),
                        specializationType: "Role",
                        specializationTypeId: RoleSpecializationTypeId,
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
