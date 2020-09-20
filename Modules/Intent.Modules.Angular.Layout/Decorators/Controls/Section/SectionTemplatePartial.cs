using Intent.Eventing;
using Intent.Modules.Angular.Layout.Api;

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
                ControlWriter.AddControl(control);
            }
        }

        public ControlWriter ControlWriter { get; }

        public SectionModel Model { get; }
    }
}
