using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.IndentedFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.IndentedFileBuilderFilePerModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IndentedFileBuilderFilePerModelTemplate : IntentTemplateBase<ClassModel>, IIndentedFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.IndentedFileBuilderFilePerModel";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IndentedFileBuilderFilePerModelTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            IndentedFile = new IndentedFile($"{Model.Name}", "txt")
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