using System.Collections.Generic;
using Intent.Eventing;
using Intent.Modules.Angular.Layout.Api;
using Intent.Modules.Angular.Templates;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.PaginationControl
{
    public partial class PaginationControlTemplate
    {
        public PaginationControlTemplate(PaginationControlModel model, IApplicationEventDispatcher eventDispatcher)
        {
            Model = model;
            eventDispatcher.Publish(AngularImportDependencyRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { AngularImportDependencyRequiredEvent.ModuleId, Model.Module.Id},
                { AngularImportDependencyRequiredEvent.Dependency, "PaginationModule.forRoot()"},
                { AngularImportDependencyRequiredEvent.Import, "import { PaginationModule } from 'ngx-bootstrap/pagination';"}
            });
        }

        public PaginationControlModel Model { get; }
    }
}
