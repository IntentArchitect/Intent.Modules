using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.Templates.AIStaticContent
{
    public abstract class AIStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;

        public AIStaticContentTemplateRegistration(string templateId, IApplicationConfigurationProvider applicationConfigurationProvider) : base(templateId)
        {
            _applicationConfigurationProvider = applicationConfigurationProvider;
        }

        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            if (fileFullPath.EndsWith(".md"))
            {
                return new AIMarkdownContentTemplate(fileFullPath, fileRelativePath, RelativeOutputPathPrefix, TemplateId, outputTarget, Replacements(outputTarget), defaultOverwriteBehaviour);
            }
            return base.CreateTemplate(outputTarget, fileFullPath, fileRelativePath, defaultOverwriteBehaviour);
        }

    }
}
