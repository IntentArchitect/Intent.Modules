using Intent.Exceptions;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modelers.CodebaseStructure.OutputTargets;

public static class OutputTargetRegistrationExtensions
{
    public static IEnumerable<FolderModel> DetectDuplicates(this IEnumerable<FolderModel> sequence)
    {
        var folderNameSet = new HashSet<string>();

        foreach (var folderModel in sequence)
        {
            if (!folderNameSet.Add(folderModel.Name))
            {
                throw new ElementException(folderModel.InternalElement, $"Duplicate Folder found at same location.");
            }
            yield return folderModel;
        }
    }
}
