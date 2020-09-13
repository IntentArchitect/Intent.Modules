using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Decorators.PaginatedSearchLayout
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class PaginatedSearchLayoutHtmlTemplateDecorator : IntentTemplateBase, IOverwriteDecorator
    {
        public const string DecoratorId = "Angular.Layout.AngularComponentHtmlDecorator.PagedSearchDecorator";
        private readonly AngularComponentHtmlTemplate _template;

        public PaginatedSearchLayoutHtmlTemplateDecorator(AngularComponentHtmlTemplate template)
        {
            _template = template;
        }

        public ComponentViewModel View => _template.Model.Views.FirstOrDefault();
        public ComponentModel Model => _template.Model;
        public ComponentModelModel PaginationModel => new ComponentModelModel(View.GetStereotypeProperty<IElement>("Search View Settings", "Pagination Model"));
        public IElement DataModel => PaginationModel.TypeReference.GenericTypeParameters.First().Element;

        public int Priority { get; } = 0;
        public string GetOverwrite() => MustOverwrite() ? TransformText() : null;
        private bool MustOverwrite() => false;// View?.TypeReference.Element.Name == "Search View";
    }
}