using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.Modules.ModuleBuilder.Templates.DesignerConfig
{
    public class DesignerConfigTemplate : IntentFileTemplateBase<DesignerModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.DesignerConfig";

        public DesignerConfigTemplate(IOutputTarget project, DesignerModel model) : base(TemplateId, project, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new MetadataRegistrationRequiredEvent(
                id: Model.Id,
                targets: new List<(string Id, string Name)>(),
                path: GetMetadata().GetFilePath()));
            //Project.Application.EventDispatcher.Publish("MetadataRegistrationRequired", new Dictionary<string, string>()
            //{
            //    { "Id", Model.Id },
            //    { "Target", Model.Name },
            //    { "Path", GetMetadata().GetRelativeFilePathWithFileNameWithExtension() },
            //});
        }

        public override string TransformText()
        {
            var designer = new ApplicationDesignerPersistable {
                Id = Model.Id,
                Name = Model.Name,
                Order = Model.GetDesignerConfig().DisplayOrder() ?? 0,
                Icon = Model.GetDesignerConfig().Icon().ToPersistable()
            };

            if (Model.GetOutputConfiguration() != null)
            {
                designer.OutputConfiguration = new DesignerOutputConfiguration()
                {
                    PackageTypeId = Model.GetOutputConfiguration().PackageType().Id,
                    RoleTypeId = Model.GetOutputConfiguration().RoleType().Id,
                    TemplateOutputTypeId = Model.GetOutputConfiguration().TemplateOutputType().Id,
                    FolderTypeId = Model.GetOutputConfiguration().FolderType()?.Id,
                };
            }

            foreach (var designerReference in Model.DesignerReferences)
            {
                designer.DesignerReferences.Add(designerReference.ToPersistable());
            }

            return Serialize(designer);
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}",
                fileExtension: ApplicationDesignerPersistable.FileExtension,
                relativeLocation: "");
        }

        private static T LoadAndDeserialize<T>(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }

        private static string Serialize<T>(T @object)
        {
            using var stringWriter = new Utf8StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(T));

            var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            });

            var xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, @object, xmlSerializerNamespaces);

            stringWriter.Close();
            return stringWriter.ToString();
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}