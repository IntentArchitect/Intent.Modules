using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.IndentedFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.IndentedFileBuilderSingle
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IndentedFileBuilderSingleTemplate : IntentTemplateBase<object>, IIndentedFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.IndentedFileBuilderSingle";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IndentedFileBuilderSingleTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            IndentedFile = new IndentedFile($"IndentedFileBuilderSingle", "txt")
                .WithItems(items =>
                {
                    items.WithContent("// Sample JSON:");
                    items.WithContent("{");
                    items.WithItems(i =>
                    {
                        i.WithContent("\"fieldName\": \"value\"");
                    });
                    items.WithContent("}");
                });
        }

        [IntentManaged(Mode.Fully)]
        public IIndentedFile IndentedFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => IndentedFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => IndentedFile.ToString();
    }
}