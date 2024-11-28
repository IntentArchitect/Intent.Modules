using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.DesignerSettings
{
    public class DesignerSettingsTemplate : IntentFileTemplateBase<DesignerSettingsModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.DesignerSettings";

        public DesignerSettingsTemplate(IOutputTarget project, DesignerSettingsModel model) : base(TemplateId, project, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new MetadataRegistrationRequiredEvent(
                Model.Id,
                Model.GetDesignerSettings().ExtendDesigners()?.Select(x => (x.Id, x.Name)).ToList() ?? new List<(string Id, string Name)>(),
                GetMetadata().GetFilePath()));
            //Project.Application.EventDispatcher.Publish("MetadataRegistrationRequired", new Dictionary<string, string>()
            //{
            //    { "Id", Model.Id },
            //    { "Target", (Model as DesignerExtensionModel)?.TypeReference.Element?.Name },
            //    { "Path", GetMetadata().GetRelativeFilePathWithFileNameWithExtension() },
            //});
        }

        public override string TransformText()
        {
            var modelerSettings = Model.ToPersistable();

            return Serialize(modelerSettings);
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}",
                fileExtension: DesignerSettingsPersistable.FileExtension,
                relativeLocation: "");
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