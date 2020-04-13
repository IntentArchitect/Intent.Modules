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
using Intent.Modules.ModelerBuilder;
using Intent.Modules.ModelerBuilder.External;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.Modules.ModuleBuilder.Templates.ModelerConfig
{
    public class ModelerConfig : IntentProjectItemTemplateBase<DesignerModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";

        public ModelerConfig(IProject project, DesignerModel model) : base(TemplateId, project, model)
        {
        }

        public override string TransformText()
        {
            var path = FileMetadata.GetFullLocationPathWithFileName();
            var applicationModelerModeler = File.Exists(path)
                ? LoadAndDeserialize<ApplicationModelerModel>(path)
                : new ApplicationModelerModel { Settings = new ModelerSettingsPersistable() };

            var modelerSettings = applicationModelerModeler.Settings;

            //modelerSettings.DiagramSettings // TODO
            modelerSettings.PackageSettings = Model.PackageSettings.ToPersistable();
            modelerSettings.ElementSettings = Model.ElementTypes.OrderBy(x => x.Name).Select(x => x.ToPersistable()).ToList();
            modelerSettings.AssociationSettings = GetAssociationSettings(Model.AssociationTypes);
            modelerSettings.StereotypeSettings = GetStereotypeSettings(Model);

            return Serialize(applicationModelerModeler);
        }

        private IconModelPersistable GetIcon(AssociationSettingsExtensions.IconFull icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
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

        private List<AssociationSettingsPersistable> GetAssociationSettings(IList<AssociationSettingsModel> associationSettings)
        {
            return associationSettings.OrderBy(x => x.Name).Select(x => new AssociationSettingsPersistable
            {
                SpecializationType = x.Name,
                Icon = GetIcon(x.GetIconFull()),
                SourceEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.SourceEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = x.SourceEnd.GetSettings().IsCollectionDefault(),
                    IsCollectionEnabled = x.SourceEnd.GetSettings().IsCollectionEnabled(),
                    IsNavigableDefault = x.SourceEnd.GetSettings().IsNavigableEnabled(),
                    IsNavigableEnabled = x.SourceEnd.GetSettings().IsNavigableEnabled(),
                    IsNullableDefault = x.SourceEnd.GetSettings().IsNullableDefault(),
                    IsNullableEnabled = x.SourceEnd.GetSettings().IsNullableEnabled()
                },
                TargetEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.DestinationEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = x.DestinationEnd.GetSettings().IsCollectionDefault(),
                    IsCollectionEnabled = x.DestinationEnd.GetSettings().IsCollectionEnabled(),
                    IsNavigableDefault = x.DestinationEnd.GetSettings().IsNavigableEnabled(),
                    IsNavigableEnabled = x.DestinationEnd.GetSettings().IsNavigableEnabled(),
                    IsNullableDefault = x.DestinationEnd.GetSettings().IsNullableDefault(),
                    IsNullableEnabled = x.DestinationEnd.GetSettings().IsNullableEnabled()
                },
            }).ToList();
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}.modeler{(Model.GetModelerSettings().ModelerType().IsExtension() ? ".extension" : "")}",
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