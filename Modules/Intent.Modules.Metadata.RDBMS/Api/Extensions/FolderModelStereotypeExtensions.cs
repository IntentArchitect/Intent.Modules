using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static Schema GetSchema(this FolderModel model)
        {
            var stereotype = model.GetStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322");
            return stereotype != null ? new Schema(stereotype) : null;
        }


        public static bool HasSchema(this FolderModel model)
        {
            return model.HasStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322");
        }

        public static bool TryGetSchema(this FolderModel model, out Schema stereotype)
        {
            if (!HasSchema(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Schema(model.GetStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322"));
            return true;
        }

        public class Schema
        {
            private IStereotype _stereotype;

            public Schema(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

    }
}