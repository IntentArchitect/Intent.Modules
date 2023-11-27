using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.DataFileBuilderFilePerModelOcl
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DataFileBuilderFilePerModelOclTemplate : IntentTemplateBase<ClassModel>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.DataFileBuilderFilePerModelOcl";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataFileBuilderFilePerModelOclTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            DataFile = new DataFile($"{Model.Name}")
                .WithOclWriter()
                .WithRootObject(this, @object =>
                {
                    @object
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