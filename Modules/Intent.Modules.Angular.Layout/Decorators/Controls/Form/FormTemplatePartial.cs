using System.Collections.Generic;
using System.Linq;
using Intent.Eventing;
using Intent.Modules.Angular.Layout.Api;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Common;

namespace Intent.Modules.Angular.Layout.Decorators.Controls.Form
{
    public partial class FormTemplate
    {
        public FormTemplate(FormModel model, IApplicationEventDispatcher eventDispatcher)
        {
            Model = model;
            eventDispatcher.Publish(AngularImportDependencyRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { AngularImportDependencyRequiredEvent.ModuleId, Model.Module.Id},
                { AngularImportDependencyRequiredEvent.Dependency, "ReactiveFormsModule"},
                { AngularImportDependencyRequiredEvent.Import, "import { ReactiveFormsModule } from '@angular/forms';"}
            });
            eventDispatcher.Publish(AngularImportDependencyRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { AngularImportDependencyRequiredEvent.ModuleId, Model.Module.Id},
                { AngularImportDependencyRequiredEvent.Dependency, "FormsModule"},
                { AngularImportDependencyRequiredEvent.Import, "import { FormsModule } from '@angular/forms';"}
            });
            if (Model.FormFields.Any(x => x.TypeReference.Element.Name == "Datepicker"))
            {
                eventDispatcher.Publish(AngularImportDependencyRequiredEvent.EventId, new Dictionary<string, string>()
                {
                    { AngularImportDependencyRequiredEvent.ModuleId, Model.Module.Id},
                    { AngularImportDependencyRequiredEvent.Dependency, "BsDatepickerModule.forRoot()"},
                    { AngularImportDependencyRequiredEvent.Import, "import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';"}
                });
            }
        }

        public FormModel Model { get; }
    }
}
