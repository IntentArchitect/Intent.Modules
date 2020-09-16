using Intent.Modules.Angular.Layout.Api;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.TableControl
{
    public partial class TableControlTemplate
    {
        public TableControlTemplate(TableControlModel model)
        {
            Model = model;
        }

        public TableControlModel Model { get; }
    }
}
