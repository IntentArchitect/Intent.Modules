using Intent.Eventing;
using Intent.Modules.Angular.Layout.Api;
using Intent.Utils;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.Section
{
    public partial class SectionTemplate
    {
        public SectionTemplate(SectionModel model, IApplicationEventDispatcher eventDispatcher)
        {
            Model = model;
            ControlWriter = new ControlWriter(eventDispatcher);
            foreach (var control in Model.InternalElement.ChildElements)
            {
                var successful = ControlWriter.AddControl(control);
                if (!successful)
                {
                    Logging.Log.Warning("Control could not be added as it has invalid bindings: " + control);
                }
            }
        }

        public ControlWriter ControlWriter { get; }

        public SectionModel Model { get; }
    }
}
