using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Documentation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DocumentationTemplate : IntentTemplateBase<ModuleDocumentationModel>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.DocumentationTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DocumentationTemplate(IOutputTarget outputTarget, ModuleDocumentationModel model) : base(TemplateId, outputTarget, model)
        {
            DataFile = new DataFile($"{Model.Name.ToLower()}")
                .WithJsonWriter()
                .WithRootObject(this, @object =>
                {
                    @object.WithArray("topics", topics =>
                    {
                        foreach (var topicModel in Model.Topics)
                        {
                            topics.WithObject(topic =>
                            {
                                topic.WithValue("title", topicModel.Name);
                                topic.WithValue("description", topicModel.GetSettings().Description());
                                topic.WithValue("iconUrl", topicModel.GetSettings().Icon()?.Source ?? ExecutionContext.GetApplicationConfig().Icon.Source);
                                topic.WithValue("href", topicModel.GetSettings().Href());
                                if (topicModel.GetSettings().Designers().Any() || topicModel.GetSettings().Elements().Any())
                                {
                                    topic.WithArray("contextIds", @array =>
                                    {
                                        foreach (var designer in topicModel.GetSettings().Designers())
                                        {
                                            @array.WithValue(designer.Id);
                                        }

                                        foreach (var element in topicModel.GetSettings().Elements())
                                        {
                                            @array.WithValue(element.Id);
                                        }
                                    });
                                }
                                if (topicModel.GetSettings().Tags() != null)
                                {
                                    topic.WithArray("tags", @array =>
                                    {
                                        foreach (var tag in topicModel.GetSettings().Tags().Split(" "))
                                        {
                                            @array.WithValue(tag);
                                        }
                                    });
                                }
                            });
                        }
                    });
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