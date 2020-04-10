using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using System;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class FolderExtensions
    {
        [IntentManaged(Mode.Ignore)]
        public static IList<FolderModel> GetFolderPath(this IHasFolder model, bool includePackage = false)
        {
            List<FolderModel> result = new List<FolderModel>();

            var current = model.Folder;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.ParentFolder;
            }
            return result;
        }

        [IntentManaged(Mode.Ignore)]
        public static IStereotype GetStereotypeInFolders(this IHasFolder model, string stereotypeName)
        {
            var folder = model.Folder;
            while (folder != null)
            {
                if (folder.HasStereotype(stereotypeName))
                {
                    return folder.GetStereotype(stereotypeName);
                }
                folder = folder.ParentFolder;
            }
            return null;
        }

    }
}