using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.DataFileBuilderSingleOcl
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DataFileBuilderSingleOclTemplate : IntentTemplateBase<object>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.DataFileBuilderSingleOcl";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataFileBuilderSingleOclTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            DataFile = new DataFile($"DataFileBuilderSingleOcl")
                .WithOclWriter()
                .WithRootDictionary(this, dictionary =>
                {
                    dictionary
                        .WithValue("fieldName", "fieldValue")
                    ;
                });
        }

        [IntentManaged(Mode.Fully)]
        public IDataFile DataFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => DataFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => DataFile.ToString();
    }
}