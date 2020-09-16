using Intent.Modules.Angular.Layout.Api;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.Section
{
    public partial class SectionTemplate
    {
        public SectionTemplate(SectionModel model)
        {
            Model = model;
        }

        public SectionModel Model { get; }
    }
}
