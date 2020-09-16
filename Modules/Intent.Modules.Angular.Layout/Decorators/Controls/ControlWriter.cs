using Intent.Metadata.Models;
using Intent.Modules.Angular.Layout.Api;
using Intent.Modules.Angular.Layout.Decorators.Controls.ButtonControl;
using Intent.Modules.Angular.Layout.Decorators.Controls.Form;
using Intent.Modules.Angular.Layout.Decorators.Controls.PaginationControl;
using Intent.Modules.Angular.Layout.Decorators.Controls.Section;
using Intent.Modules.Angular.Layout.Decorators.Controls.TableControl;

namespace Intent.Modules.Angular.Layout.Decorators.Controls
{
    public static class ControlWriter
    {
        public static string WriteControl(IElement control)
        {
            switch (control.SpecializationType)
            {
                case SectionModel.SpecializationType:
                    return new SectionTemplate(new SectionModel(control)).TransformText();
                case ButtonControlModel.SpecializationType:
                    return new ButtonControlTemplate(new ButtonControlModel(control)).TransformText();
                case TableControlModel.SpecializationType:
                    {
                        var model = new TableControlModel(control);
                        if (model.IsValid())
                        {
                            return new TableControlTemplate(model).TransformText();
                        }

                        break;
                    }
                case PaginationControlModel.SpecializationType:
                    {
                        var model = new PaginationControlModel(control);
                        if (model.IsValid())
                        {
                            return new PaginationControlTemplate(model).TransformText();
                        }

                        break;
                    }
                case FormModel.SpecializationType:
                    {
                        var model = new FormModel(control);
                        if (model.IsValid())
                        {
                            return new FormTemplate(model).TransformText();
                        }

                        break;
                    }
            }
            return "";
        }
    }
}