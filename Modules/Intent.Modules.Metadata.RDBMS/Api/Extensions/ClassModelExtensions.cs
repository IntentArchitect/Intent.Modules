using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [Obsolete("Replaced by ClassModelStereotypeExtensions")]
    public static class ClassModelExtensions
    {
        [Obsolete]
        public static Table GetTable(ClassModel model)
        {
            var stereotype = model.GetStereotype("Table");
            return stereotype != null ? new Table(stereotype) : null;
        }

        [Obsolete]
        public static bool HasTable(ClassModel model)
        {
            return model.HasStereotype("Table");
        }


        [Obsolete]
        public class Table
        {
            private IStereotype _stereotype;

            public Table(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string Schema()
            {
                return _stereotype.GetProperty<string>("Schema");
            }

        }

    }
}