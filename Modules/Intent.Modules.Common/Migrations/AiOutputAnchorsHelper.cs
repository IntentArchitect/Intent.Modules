using System;
using System.Linq;
using Intent.Persistence.V2;

namespace Intent.Modules.Common.Migrations
{
    /// <summary>
    /// To simplify variable and member names, the term "anchor" can be used interchangeably with "role".
    /// </summary>
    internal static class AiOutputAnchorsHelper
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string FolderStructureDesignerId = "27916ede-58c6-4356-87b1-0b0628d1e2a7";
        public const string AnchorType = "Output Anchor";
        public const string AnchorTypeId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        public const string RoleType = "Role";
        public const string RoleTypeId = "8663c9f9-2852-45e7-aaa1-0883a2e6f1da";
        public const string FolderType = "Folder";
        public const string FolderTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";

        public static void EnsureExist(IApplicationPersistable application)
        {
            var codebaseStructureDesigner = application.GetDesigners().SingleOrDefault(d => d.Id == CodebaseStructureDesignerId);
            var folderStructureDesigner = application.GetDesigners().SingleOrDefault(d => d.Id == FolderStructureDesignerId);

            if (codebaseStructureDesigner != null)
            {
                AddAnchorsMaybe(
                    designer: codebaseStructureDesigner,
                    anchorType: AnchorType,
                    anchorTypeId: AnchorTypeId);
            }
            else if (folderStructureDesigner != null)
            {
                AddAnchorsMaybe(
                    designer: folderStructureDesigner,
                    anchorType: RoleType,
                    anchorTypeId: RoleTypeId);
            }
        }

        private static void AddAnchorsMaybe(
            IApplicationDesignerPersistable designer,
            string anchorType,
            string anchorTypeId)
        {
            var package = designer?.GetPackages().FirstOrDefault();
            if (package == null)
            {
                return;
            }

            var needsSave = false;

            needsSave |= package.EnsureAnchorExists(
                name: "AI.Context",
                defaultContainingFolderName: ".agents",
                defaultContainingFolderParentId: package.Id,
                anchorType: anchorType,
                anchorTypeId: anchorTypeId,
                anchorElement: out var aiContextAnchor);

            needsSave |= package.EnsureAnchorExists(
                name: "AI.Context.Instructions",
                defaultContainingFolderName: "instructions",
                defaultContainingFolderParentId: aiContextAnchor.ParentFolderId,
                anchorType: anchorType,
                anchorTypeId: anchorTypeId,
                anchorElement: out _);

            needsSave |= package.EnsureAnchorExists(
                name: "AI.Context.Skills",
                defaultContainingFolderName: "skills",
                defaultContainingFolderParentId: aiContextAnchor.ParentFolderId,
                anchorType: anchorType,
                anchorTypeId: anchorTypeId,
                anchorElement: out _);

            if (needsSave)
            {
                package.Save();
            }
        }

        /// <summary>
        /// To simplify variable and member names, the term "anchor" can be used interchangeable with "role".
        /// </summary>
        /// <returns>Whether a save is needed</returns>
        private static bool EnsureAnchorExists(
            this IPackageModelPersistable package,
            string name,
            string defaultContainingFolderName,
            string defaultContainingFolderParentId,
            string anchorType,
            string anchorTypeId,
            out IElementPersistable anchorElement)
        {
            anchorElement = package.Classes.FirstOrDefault(x => x.Name == name && x.SpecializationTypeId == anchorTypeId);
            if (anchorElement != null)
            {
                return false;
            }

            var folder = package.GetOrCreateFolder(defaultContainingFolderParentId, defaultContainingFolderName);

            anchorElement = package.Classes.Add(
                id: Guid.NewGuid().ToString(),
                specializationType: anchorType,
                specializationTypeId: anchorTypeId,
                name: name,
                parentId: folder.Id);
            return true;
        }

        private static IElementPersistable GetOrCreateFolder(this IPackageModelPersistable package, string parentId, string name)
        {
            var folder = package.Classes.SingleOrDefault(x =>
                x.SpecializationTypeId == FolderTypeId &&
                x.Name == name &&
                x.ParentFolderId == parentId);

            return folder ?? package.Classes.Add(
                id: Guid.NewGuid().ToString(),
                specializationType: FolderType,
                specializationTypeId: FolderTypeId,
                name: name,
                parentId: parentId);
        }
    }
}
