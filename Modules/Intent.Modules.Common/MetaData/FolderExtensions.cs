using System.Collections.Generic;
using Intent.MetaModel.Common;

namespace Intent.Modules.Common
{
    public static class FolderExtensions
    {
        public static IList<IFolder> GetFolderPath(this IHasFolder model, bool includePackage = true)
        {
            List<IFolder> result = new List<IFolder>();

            var current = model.Folder;
            while (current != null && (includePackage || !current.IsPackage))
            {
                result.Insert(0, current);
                current = current.ParentFolder;
            }
            return result;
        }
    }
}
