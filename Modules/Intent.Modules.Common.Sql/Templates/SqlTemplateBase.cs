﻿using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Sql.Templates
{
    public abstract class SqlTemplateBase<TModel> : IntentTemplateBase<TModel>, ISqlTemplate
    {
        protected SqlTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new SqlTypeResolver();
        }

        public string Location => FileMetadata.LocationInProject;

        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();

            return templateOutput;
        }
    }

    public interface ISqlTemplate
    {
    }

    public class SqlFileConfig : TemplateFileConfig
    {
        public SqlFileConfig(
                    OverwriteBehaviour overwriteBehaviour,
                    string fileName,
                    string relativeLocation,
                    string fileExtension = "sql",
                    string codeGenType = Common.CodeGenType.Basic
                    )
            : base(overwriteBehaviour: overwriteBehaviour,
                  codeGenType: codeGenType,
                  fileName: fileName,
                  fileExtension: fileExtension,
                  relativeLocation: relativeLocation)
        {
        }
    }
}