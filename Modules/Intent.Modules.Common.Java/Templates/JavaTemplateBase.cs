using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Java.Editor;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    public abstract class JavaTemplateBase : JavaTemplateBase<object>
    {
        protected JavaTemplateBase(string templateId, IProject outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class JavaTemplateBase<TModel> : IntentProjectItemTemplateBase<TModel>, IJavaMerged
    {
        protected JavaTemplateBase(string templateId, IProject outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        public string Namespace
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"];
                }
                return null;
            }
        }

        public string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"];
                }
                return FileMetadata.FileName;
            }
        }

        public string Location => FileMetadata.LocationInProject;


        public override string RunTemplate()
        {
            var file = CreateOutputFile();

            //file.AddDependencyImports(this);

            return file.GetSource();
        }

        protected virtual JavaFile CreateOutputFile()
        {
            return GetTemplateFile();
        }

        public JavaFile GetTemplateFile()
        {
            return new JavaFile(base.RunTemplate());
        }

        public JavaFile GetExistingFile()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
            return File.Exists(fullFileName) ? new JavaFile(File.ReadAllText(fullFileName)) : null;
        }
    }
}
