using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Component.Layouts.PaginatedSearchLayout
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class PaginatedSearchLayoutHtmlTemplateDecorator : IOverwriteDecorator
    {
        public const string DecoratorId = "Angular.AngularComponentHtmlTemplate.PagedSearchDecorator";
        private readonly AngularComponentHtmlTemplate.AngularComponentHtmlTemplate _template;
        private readonly IMetadataManager _metadataManager;
        private readonly ITypeReference _pagingModelReference;

        public PaginatedSearchLayoutHtmlTemplateDecorator(AngularComponentHtmlTemplate.AngularComponentHtmlTemplate template,
            IMetadataManager metadataManager)
        {
            _template = template;
            _metadataManager = metadataManager;
            _pagingModelReference = _template.Model.Models.FirstOrDefault(x => x.Name == _template.Model.GetStereotypeProperty("Paginated Search Layout", "Paging Model", ""))?.TypeReference;
        }

        public ComponentViewModel View => _template.Model.Views.FirstOrDefault();
        public ComponentModel Model => _template.Model;
        public ComponentModelModel PaginationModel => new ComponentModelModel(View.GetStereotypeProperty<IElement>("Search View Settings", "Pagination Model"));
        public ModelDefinitionModel DataModel => new ModelDefinitionModel(PaginationModel.TypeReference.GenericTypeParameters.First().Element);

        public int Priority { get; } = 0;
        public string GetOverwrite() => MustOverwrite() ? TransformText() : null;

        private bool MustOverwrite() => View?.TypeReference.Element.Name == "Search View";
    }

    public interface IOverwriteDecorator : ITemplateDecorator
    {
        string GetOverwrite();
    }
}