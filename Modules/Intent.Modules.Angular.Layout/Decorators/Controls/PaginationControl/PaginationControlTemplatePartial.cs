using Intent.Modules.Angular.Layout.Api;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.PaginationControl
{
    public partial class PaginationControlTemplate
    {
        public PaginationControlTemplate(PaginationControlModel model)
        {
            Model = model;
        }

        public PaginationControlModel Model { get; }
    }
}
