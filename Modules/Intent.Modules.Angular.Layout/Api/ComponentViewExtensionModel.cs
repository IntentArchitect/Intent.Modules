using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Api
{
    [IntentManaged(Mode.Merge)]
    public class ComponentViewExtensionModel : ComponentViewModel
    {
        [IntentManaged(Mode.Ignore)]
        public ComponentViewExtensionModel(IElement element) : base(element)
        {
        }

        public IList<TableControlModel> TableControls => _element.ChildElements
            .Where(x => x.SpecializationType == TableControlModel.SpecializationType)
            .Select(x => new TableControlModel(x))
            .ToList();

        public IList<PaginationControlModel> PaginationControls => _element.ChildElements
            .Where(x => x.SpecializationType == PaginationControlModel.SpecializationType)
            .Select(x => new PaginationControlModel(x))
            .ToList();

        public IList<FormModel> Forms => _element.ChildElements
            .Where(x => x.SpecializationType == FormModel.SpecializationType)
            .Select(x => new FormModel(x))
            .ToList();

        public IList<ButtonControlModel> ButtonControls => _element.ChildElements
            .Where(x => x.SpecializationType == ButtonControlModel.SpecializationType)
            .Select(x => new ButtonControlModel(x))
            .ToList();

        public IList<SectionModel> Sections => _element.ChildElements
            .Where(x => x.SpecializationType == SectionModel.SpecializationType)
            .Select(x => new SectionModel(x))
            .ToList();

        public IList<TabsModel> Tabses => _element.ChildElements
            .Where(x => x.SpecializationType == TabsModel.SpecializationType)
            .Select(x => new TabsModel(x))
            .ToList();

    }
}