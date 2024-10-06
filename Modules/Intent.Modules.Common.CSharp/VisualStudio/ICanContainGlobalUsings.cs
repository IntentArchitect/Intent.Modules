﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    /// <summary>
    /// Used to indicates the template and/or its existing file can contain global usings.
    /// </summary>
    public interface ICanContainGlobalUsings
    {
        /// <summary>
        /// Returns the global usings generated by the file as well as any global usings
        /// of the existing file.
        /// </summary>
        IEnumerable<string> GetGlobalUsings()
        {
            if (this is ICSharpFileBuilderTemplate cSharpFileBuilderTemplate)
            {
                foreach (var @using in cSharpFileBuilderTemplate.CSharpFile.Usings.Where(x => x.IsGlobal))
                {
                    yield return @using.Namespace;
                }
            }

            if (this is not IIntentTemplate intentTemplate ||
                Path.GetExtension(intentTemplate.FileMetadata.GetFilePath())?.Equals(".cs", StringComparison.OrdinalIgnoreCase) != true ||
                !intentTemplate.TryGetExistingFileContent(out var content))
            {
                yield break;
            }

            var lines = content.ReplaceLineEndings("\n").Split("\n");
            foreach (var line in lines)
            {
                if (!line.Trim().StartsWith("global using "))
                {
                    continue;
                }

                yield return line["global using ".Length..^1];
            }
        }
    }
}
