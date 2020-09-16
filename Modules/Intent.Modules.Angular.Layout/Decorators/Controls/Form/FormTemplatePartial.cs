using Intent.Modules.Angular.Layout.Api;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.Form
{
    public partial class FormTemplate
    {
        public FormTemplate(FormModel model)
        {
            Model = model;
        }

        public FormModel Model { get; }
    }
}
