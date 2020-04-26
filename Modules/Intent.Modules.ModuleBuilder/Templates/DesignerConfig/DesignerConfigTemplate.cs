using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModelerBuilder.External;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.Modules.ModuleBuilder.Templates.DesignerConfig
{
    public class DesignerConfigTemplate : IntentProjectItemTemplateBase<DesignerModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";

        public DesignerConfigTemplate(IProject project, DesignerModel model) : base(TemplateId, project, model)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Project.Application.EventDispatcher.Publish("MetadataRegistrationRequired", new Dictionary<string, string>()
            {
                { "Target", Model.Name },
                { "Folder", GetMetadata().LocationInProject }
            });
        }

        public override string TransformText()
        {
            var path = FileMetadata.GetFullLocationPathWithFileName();
            var applicationModelerModeler = File.Exists(path)
                ? LoadAndDeserialize<ApplicationModelerModel>(path)
                : new ApplicationModelerModel { Settings = new ModelerSettingsPersistable() };

            applicationModelerModeler.Icon = Model.GetDesignerSettings().Icon().ToPersistable();
            applicationModelerModeler.DisplayOrder = Model.GetDesignerSettings().DisplayOrder() ?? 0;
            var modelerSettings = applicationModelerModeler.Settings;

            //modelerSettings.DiagramSettings // TODO
            modelerSettings.PackageSettings = Model.PackageSettings?.ToPersistable();
            modelerSettings.ElementSettings = Model.ElementTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList();
            modelerSettings.AssociationSettings = Model.AssociationTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList();
            modelerSettings.ElementExtensions = (Model as DesignerExtensionModel)?.ElementExtensions.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList();

            modelerSettings.StereotypeSettings = GetStereotypeSettings(Model);

            return Serialize(applicationModelerModeler);
        }


        private StereotypeSettingsPersistable GetStereotypeSettings(DesignerModel model)
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
                fileName: $"{Model.Name}.modeler{(Model is DesignerExtensionModel ? ".extension" : "")}",
                fileExtension: "config",
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