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
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using StereotypePropertyControlType = Intent.IArchitect.Agent.Persistence.Model.Common.StereotypePropertyControlType;
using StereotypePropertyOptionsSource = Intent.IArchitect.Agent.Persistence.Model.Common.StereotypePropertyOptionsSource;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.metadata.StereotypeDefinitionFileTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StereotypeDefinitionFileTemplate : IntentProjectItemTemplateBase<IStereotypeDefinition>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.metadata.StereotypeDefinitionFileTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        public StereotypeDefinitionFileTemplate(IProject project, IStereotypeDefinition model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name} [${Model.Id}]",
                fileExtension: "xml",
                defaultLocationInProject: $"metadata/{Model.GetParentPath().Single(x => x.SpecializationType == ModulePackage.SpecializationType).Name}/Stereotypes"
            );
        }

        public override string TransformText()
        {
            //var path = FileMetadata.GetFullLocationPathWithFileName();
            //var stereotypeDefinition = File.Exists(path)
            //    ? LoadAndDeserialize<StereotypeDefinitionPersistable>(path)
            //    : new StereotypeDefinitionPersistable();

            var stereotypeDefinition = new StereotypeDefinitionPersistable()
            {
                Id = Model.Id,
                Name = Model.Name,
                Icon = new IconModelPersistable() { Type = Enum.Parse<Intent.IArchitect.Common.Types.IconType>(Model.Icon.Type.ToString()), Source = Model.Icon.Source },
                DisplayFunction = Model.DisplayFunction,
                Comment = Model.Comment,
                TargetElements = Model.TargetElements.ToList(),
                AutoAdd = Model.AutoAdd,
                DisplayIcon = Model.DisplayIcon,
                ParentFolderId = Model.ParentElement.Id,
                Properties = Model.Properties.Select(p => new StereotypePropertyDefinitionPersistable()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ControlType = ((StereotypePropertyControlType)p.ControlType),
                        OptionsSource = ((StereotypePropertyOptionsSource?)p.OptionsSource) ?? StereotypePropertyOptionsSource.NotApplicable,
                        ValueOptions = p.ValueOptions.ToList(),
                        LookupInternalTargetPropertyId = p.LookupInternalTargetPropertyId,
                        LookupTypes = p.LookupTypes.ToList(),
                        DefaultValue = p.DefaultValue,
                        Placeholder = p.Placeholder,
                        IsActiveFunction = p.IsActiveFunction,
                        IsRequiredFunction = p.IsRequiredFunction,
                        Comment = p.Comment
                    })
                    .ToList()
            };

            return Serialize(stereotypeDefinition);
        }

        private string FolderPath => string.Join("/", Model.GetParentPath()
            .Reverse()
            .TakeWhile(x => x.SpecializationType != MetadataFolder.SpecializationType)
            .Reverse()
            .Select(x => x.Name));

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