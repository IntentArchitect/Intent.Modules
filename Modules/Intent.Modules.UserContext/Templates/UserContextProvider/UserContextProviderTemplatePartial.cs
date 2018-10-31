using Intent.Modules.Constants;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.UserContext.Templates.UserContextProviderInterface;

namespace Intent.Modules.UserContext.Templates.UserContextProvider
{
    partial class UserContextProviderTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasTemplateDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.UserContext.UserContextProvider";

        private readonly IApplicationEventDispatcher _eventDispatcher;

        public UserContextProviderTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, null)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                       overwriteBehaviour: OverwriteBehaviour.Always,
                       fileName: $"UserContextProvider",
                       fileExtension: "cs",
                       defaultLocationInProject: "Providers",
                       className: "UserContextProvider",
                       @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new List<ITemplateDependancy>
            {
                TemplateDependancy.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }

        public void PreProcess()
        {
            var userContextProviderInterface = Project.FindTemplateInstance<IHasClassDetails>(UserContextProviderInterfaceTemplate.Identifier);
            var contractTemplate = Project.FindTemplateInstance<IHasClassDetails>(UserContextInterfaceTemplate.Identifier);
            _eventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", $"{userContextProviderInterface.FullTypeName()}<{contractTemplate.FullTypeName()}>" },
                { "ConcreteType",  this.FullTypeName() }
            });
        }
    }
}
