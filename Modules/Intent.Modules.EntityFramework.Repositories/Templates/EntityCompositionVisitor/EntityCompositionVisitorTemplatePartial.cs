using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.EntityCompositionVisitor
{
    partial class EntityCompositionVisitorTemplate : CSharpTemplateBase<IEnumerable<ClassModel>>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.EntityCompositionVisitor";
        private ITemplateDependency[] _entityStateTemplateDependancies;

        public EntityCompositionVisitorTemplate(IEnumerable<ClassModel> models, IProject project)
            : base (Identifier, project, models)
        {

        }

        public string BoundedContextName => Project.ApplicationName();


        public override void OnCreated()
        {
            _entityStateTemplateDependancies = Model.Select(x => TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == x.Id)).ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _entityStateTemplateDependancies;
        }

        public string GetClassName(ClassModel @class)
        {
            return Project.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == @class.Id))?.ClassName ?? $"{@class.Name}";
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"EntityCompositionVisitor",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
