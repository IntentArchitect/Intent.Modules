using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Settings;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.Modules.ModuleBuilder.Templates.DesignerSettings
{
    public class DesignerSettingsTemplate : IntentProjectItemTemplateBase<DesignerSettingsModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";

        public DesignerSettingsTemplate(IProject project, DesignerSettingsModel model) : base(TemplateId, project, model)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Project.Application.EventDispatcher.Publish("MetadataRegistrationRequired", new Dictionary<string, string>()
            {
                { "Target", (Model as DesignerExtensionModel)?.TypeReference.Element?.Name ?? Model.Name },
                { "Folder", GetMetadata().LocationInProject }
            });
        }

        public override string TransformText()
        {
            var modelerSettings = new DesignerSettingsPersistable
            {
                Id = Model.Id,
                Name = Model.Name,
                PackageSettings = Model.PackageTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList(),
                PackageExtensions = Model.PackageExtensions.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList(),
                ElementSettings = Model.ElementTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList(),
                ElementExtensions = Model.ElementExtensions.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList(),
                AssociationSettings = Model.AssociationTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList(),
            };

            return Serialize(modelerSettings);
        }


        private StereotypeSettingsPersistable GetStereotypeSettings(DesignerSettingsModel model)
        {
            var targetTypes = model.ElementTypes.Select(x => x.Name)
                .Concat(model.ElementTypes.SelectMany(x => x.ElementSettings).Select(x => x.Name))
                .Concat(model.AssociationTypes.Select(x => x.Name))
                .OrderBy(x => x)
                .ToList();

            return new StereotypeSettingsPersistable
            {
                TargetTypeOptions = targetTypes.Select(x => new StereotypeTargetTypeOption()
                {
                    SpecializationType = x,
                    DisplayText = x
                }).ToList()
            };
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}",
                fileExtension: DesignerSettingsPersistable.FileExtension,
                defaultLocationInProject: "modelers");
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
            using (var stringWriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                var serializerNamespaces = new XmlSerializerNamespaces();
                serializerNamespaces.Add("", "");

                serializer.Serialize(stringWriter, @object, serializerNamespaces);
                stringWriter.Close();

                return stringWriter.ToString();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

    }
}