﻿using System;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Sql.Templates;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Sql.Weaving
{
    public class SqlWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.Sql.OutputWeaver";

        public bool CanTransform(IOutputFile output)
        {
            return output.Template is ISqlTemplate;
        }

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is ISqlTemplate))
            {
                throw new InvalidOperationException($"Cannot transform outputs where the template does not derive from {nameof(ISqlTemplate)}");
            }

            var existingFile = output.GetExistingFileContent();

            if (existingFile == null)
            {
                return;
            }

            try
            {
                var instruction = existingFile.TrimStart().Substring(0, Math.Min(50, existingFile.Length)).Replace(" ", "");
                if (instruction.StartsWith("--INTENTIGNORE", StringComparison.InvariantCultureIgnoreCase))
                {
                    output.ChangeContent(existingFile);
                }
            }
            catch (Exception e)
            {
                Logging.Log.Warning($"Error while managing SQL file: {output.FileMetadata.GetFilePath()}");
                Logging.Log.Warning(e.ToString());
            }
        }
    }
}
