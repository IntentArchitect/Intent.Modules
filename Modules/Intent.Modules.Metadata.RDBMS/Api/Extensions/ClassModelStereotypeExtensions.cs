using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{

    public static class ClassModelStereotypeExtensions
    {
        public static CheckConstraint GetCheckConstraint(this ClassModel model)
        {
            var stereotype = model.GetStereotype("Check Constraint");
            return stereotype != null ? new CheckConstraint(stereotype) : null;
        }

        public static IReadOnlyCollection<CheckConstraint> GetCheckConstraints(this ClassModel model)
        {
            var stereotypes = model
                .GetStereotypes("Check Constraint")
                .Select(stereotype => new CheckConstraint(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasCheckConstraint(this ClassModel model)
        {
            return model.HasStereotype("Check Constraint");
        }
        public static Table GetTable(this ClassModel model)
        {
            var stereotype = model.GetStereotype("Table");
            return stereotype != null ? new Table(stereotype) : null;
        }

        public static IReadOnlyCollection<Table> GetTables(this ClassModel model)
        {
            var stereotypes = model
                .GetStereotypes("Table")
                .Select(stereotype => new Table(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasTable(this ClassModel model)
        {
            return model.HasStereotype("Table");
        }


        public class CheckConstraint
        {
            private IStereotype _stereotype;

            public CheckConstraint(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string SQL()
            {
                return _stereotype.GetProperty<string>("SQL");
            }

        }


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