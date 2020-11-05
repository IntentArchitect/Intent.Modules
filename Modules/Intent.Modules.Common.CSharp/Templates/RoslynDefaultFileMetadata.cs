using System;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class CSharpDefaultFileConfig : DefaultFileMetadata
    {
        public CSharpDefaultFileConfig(
                    string className,
                    string @namespace,
                    string relativeLocation = "",
                    OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
                    string fileName = null,
                    string fileExtension = "cs",
                    string dependsUpon = null
                    )
            : base(overwriteBehaviour, "RoslynWeave", fileName ?? className, fileExtension, relativeLocation)
        {
            CustomMetadata["ClassName"] = className ?? throw new ArgumentNullException(nameof(className));
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                this.CustomMetadata["Namespace"] = @namespace;
            }
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                this.CustomMetadata["Depends On"] = dependsUpon;
            }
        }
    }

    [Obsolete("Use CSharpDefaultFileConfig")]
    public class RoslynDefaultFileMetadata : DefaultFileMetadata
    {
        [Obsolete("use the one with namespace and classname ans specify them, upgrade template to bind to ClassName and Namespace")]
        public RoslynDefaultFileMetadata(
                    OverwriteBehaviour overwriteBehaviour,
                    string fileName,
                    string fileExtension,
                    string defaultLocationInProject
                    )
            : this(overwriteBehaviour, fileName, fileExtension, defaultLocationInProject, null, null, null)
        {
            this.CustomMetadata["Namespace"] = "${Project.ProjectName}";
        }

        public RoslynDefaultFileMetadata(
                    OverwriteBehaviour overwriteBehaviour,
                    string fileName,
                    string fileExtension,
                    string defaultLocationInProject,
                    string className,
                    string @namespace,
                    string dependsUpon = null
                    )
            : base(overwriteBehaviour, "RoslynWeave", fileName, fileExtension, defaultLocationInProject)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                this.CustomMetadata["ClassName"] = className;
            }
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                this.CustomMetadata["Namespace"] = @namespace;
            }
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                this.CustomMetadata["Depends On"] = dependsUpon;
                /*
                if (!string.IsNullOrWhiteSpace(defaultLocationInProject))
                {
                    this.CustomMetadata["Depends On"] = defaultLocationInProject + "/" + dependsUpon;
                }
                else
                {
                    this.CustomMetadata["Depends On"] = dependsUpon;
                }*/
            }
        }
    }
}
