using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Layout.Api;
using Intent.Modules.Angular.Layout.Decorators.Controls.ButtonControl;
using Intent.Modules.Angular.Layout.Decorators.Controls.Form;
using Intent.Modules.Angular.Layout.Decorators.Controls.PaginationControl;
using Intent.Modules.Angular.Layout.Decorators.Controls.Section;
using Intent.Modules.Angular.Layout.Decorators.Controls.TableControl;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Angular.Layout.Decorators.Controls
{
    public class ControlWriter
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IList<T4TemplateBase> _controls = new List<T4TemplateBase>();

        public ControlWriter(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public bool AddControl(IElement control)
        {
            switch (control.SpecializationType)
            {
                case SectionModel.SpecializationType:
                    _controls.Add(new SectionTemplate(new SectionModel(control), _eventDispatcher));
                    return true;
                case ButtonControlModel.SpecializationType:
                    _controls.Add(new ButtonControlTemplate(new ButtonControlModel(control)));
                    return true;
                case TableControlModel.SpecializationType:
                    {
                        var model = new TableControlModel(control);
                        if (model.IsValid())
                        {
                            _controls.Add(new TableControlTemplate(model));
                            return true;
                        }
                        return false;
                    }
                case PaginationControlModel.SpecializationType:
                    {
                        var model = new PaginationControlModel(control);
                        if (model.IsValid())
                        {
                            _controls.Add(new PaginationControlTemplate(model, _eventDispatcher));
                            return true;
                        }
                        return false;
                    }
                case FormModel.SpecializationType:
                    {
                        var model = new FormModel(control);
                        if (model.IsValid())
                        {
                            _controls.Add(new FormTemplate(model, _eventDispatcher));
                            return true;
                        }
                        return false;
                    }
            }

            return false;
        }

        public string WriteControls()
        {
            return string.Join("", _controls.Select(x => x.TransformText()));
        }
    }
}