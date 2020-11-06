using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProviderInterface
{
    partial class UserContextProviderInterfaceTemplate : CSharpTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.UserContextProviderInterface";


        public UserContextProviderInterfaceTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                       overwriteBehaviour: OverwriteBehaviour.Always,
                       fileName: $"IUserContextProvider",
                       fileExtension: "cs",
                       relativeLocation: "Providers",
                       className: "IUserContextProvider",
                       @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new List<ITemplateDependency>
            {
                TemplateDependency.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}
