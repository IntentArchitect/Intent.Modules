using System.Linq;
using System.Text;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Layout.Decorators.Controls;
using Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Decorators.AngularComponentHtml
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AngularComponentHtmlControlsDecorator : IOverwriteDecorator
    {
        public const string DecoratorId = "Angular.Layout.AngularComponentHtmlDecorator.Controls";
        private readonly AngularComponentHtmlTemplate _template;
        private readonly ControlWriter _controlWriter;

        public AngularComponentHtmlControlsDecorator(AngularComponentHtmlTemplate template)
        {
            _template = template;
            _controlWriter = new ControlWriter(_template.ExecutionContext.EventDispatcher);
            if (View == null)
            {
                return;
            }
            foreach (var control in View.InternalElement.ChildElements)
            {
                var successful = _controlWriter.AddControl(control);
                if (!successful)
                {
                    Logging.Log.Warning("Control could not be added as it has invalid bindings: " + control);
                }
            }
        }

        public ComponentViewModel View => _template.Model.View;
        public ComponentModel Model => _template.Model;
        //public ComponentModelModel PaginationModel => new ComponentModelModel(View.GetStereotypeProperty<IElement>("Search View Settings", "Pagination Model"));
        //public IElement DataModel => PaginationModel.TypeReference.GenericTypeParameters.First().Element;

        public int Priority { get; } = 0;
        public string GetOverwrite() => MustOverwrite() ? GetOutput() : null;
        private bool MustOverwrite() => View?.InternalElement.ChildElements.Any() ?? false;

        public string GetOutput()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class=\"container-fluid\" intent-manage=\"add remove\" intent-id=\"container\">");
            sb.Append(_controlWriter.WriteControls());
            sb.AppendLine("</div>");
            return sb.ToString();
        }
    }
}