using Intent.Modules.Angular.Layout.Api;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.ButtonControl
{
    public partial class ButtonControlTemplate
    {
        public ButtonControlTemplate(ButtonControlModel model)
        {
            Model = model;
        }

        public ButtonControlModel Model { get; }
    }
}
