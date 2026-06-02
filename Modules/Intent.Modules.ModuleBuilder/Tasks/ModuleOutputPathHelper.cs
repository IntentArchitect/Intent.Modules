using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Utils;

namespace Intent.Modules.ModuleBuilder.Tasks
{
    internal static class ModuleOutputPathHelper
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string OutputAnchorTypeId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        private const string RootFolderSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string VSSolutionElementSpecializationId = "769a45a2-119f-434f-8c27-bd4a399b915c";
        private const string SolutionFolderSpecializationId = "0dc2b846-c968-49eb-99b7-8776919313a8";

        // Applies only to Solution Folder elements — controls whether they materialise on disk.
        private const string SolutionFolderOptionsStereotypeId = "bb59d570-80b6-4564-9016-f809d6eacbc5";
        private const string MaterializeFolderPropertyId = "4bcd7fb5-54e5-4812-b80c-4176114086c4";

        /// <summary>
        /// Resolves the physical directory that contains the .imodspec and release-notes.md
        /// for the given application by reading the IModSpec Output Anchor position from
        /// the Codebase Structure designer.
        /// Returns null if the application cannot be found.
        /// </summary>
        public static string ResolveIModSpecDirectory(
            IMetadataManager metadataManager,
            ISolutionConfig solution,
            string applicationId)
        {
            var appConfig = solution.GetApplicationConfig(applicationId);
            if (appConfig == null)
                return null;

            var basePath = Path.GetFullPath(
                Path.Combine(
                    appConfig.DirectoryPath.Replace("\\", "/"),
                    appConfig.OutputLocation.Replace("\\", "/")));

            try
            {
                var designer = metadataManager.GetDesigner(applicationId, CodebaseStructureDesignerId);
                if (designer == null)
                    return basePath;

                var anchor = designer.GetElementsOfType(OutputAnchorTypeId)
                    .FirstOrDefault(x => x.Name == "IModSpec");

                if (anchor == null)
                    return basePath;

                var folderNames = new List<string>();
                var current = anchor.ParentElement;
                while (current != null)
                {
                    if (ContributesToPhysicalPath(current))
                        folderNames.Insert(0, current.Name);
                    current = current.ParentElement;
                }

                return folderNames.Count == 0
                    ? basePath
                    : Path.Combine(new[] { basePath }.Concat(folderNames).ToArray());
            }
            catch (Exception e)
            {
                Logging.Log.Warning($"Could not resolve IModSpec directory from Codebase Structure, falling back to base path. ({e.Message})");
                return basePath;
            }
        }

        private static bool ContributesToPhysicalPath(IElement element)
        {
            // Solution-level containers that never map to a real directory
            if (element.SpecializationTypeId == VSSolutionElementSpecializationId)
                return false;
            if (element.SpecializationTypeId == RootFolderSpecializationId)
                return false;

            // Solution Folders (above projects) only contribute when Materialize Folder is true
            if (element.SpecializationTypeId == SolutionFolderSpecializationId)
            {
                if (!element.HasStereotype(SolutionFolderOptionsStereotypeId))
                    return false;

                var stereotype = element.GetStereotype(SolutionFolderOptionsStereotypeId);
                var value = stereotype.GetProperty(MaterializeFolderPropertyId)?.Value;
                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            // Project nodes and inside-project folders always map to a real directory
            return true;
        }
    }
}
