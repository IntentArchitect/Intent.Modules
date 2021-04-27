using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.Types.Api
{
    public static class FolderExtensions
    {
        public static IList<FolderModel> GetParentFolders(this IHasFolder model)
        {
            List<FolderModel> result = new List<FolderModel>();

            var current = model.Folder;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.Folder;
            }
            return result;
        }

        public static IList<IFolder> GetParentFolders(this IHasFolder<IFolder> model) 
        {
            List<IFolder> result = new List<IFolder>();

            IFolder current = model.Folder;
            while (current != null)
            {
                result.Insert(0, current);
                current = (current as IHasFolder<IFolder>)?.Folder;
            }
            return result;
        }

        public static IList<string> GetParentFolderNames(this IHasFolder model)
        {
            return model.GetParentFolders().Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        public static IList<string> GetParentFolderNames(this IHasFolder<IFolder> model)
        {
            return model.GetParentFolders().Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        public static IStereotype GetStereotypeInFolders(this IHasFolder model, string stereotypeName)
        {
            var folder = model.Folder;
            while (folder != null)
            {
                if (folder.HasStereotype(stereotypeName))
                {
                    return folder.GetStereotype(stereotypeName);
                }
                folder = folder.Folder;
            }
            return null;
        }

    }
}