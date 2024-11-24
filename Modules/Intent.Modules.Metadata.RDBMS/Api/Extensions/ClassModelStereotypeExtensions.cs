using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{

    public static class ClassModelStereotypeExtensions
    {
        public static CheckConstraint GetCheckConstraint(this ClassModel model)
        {
            var stereotype = model.GetStereotype(CheckConstraint.DefinitionId);
            return stereotype != null ? new CheckConstraint(stereotype) : null;
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetCheckConstraint"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        [IntentManaged(Mode.Ignore)]
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
            return model.HasStereotype(CheckConstraint.DefinitionId);
        }

        public static bool TryGetCheckConstraint(this ClassModel model, out CheckConstraint stereotype)
        {
            if (!HasCheckConstraint(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CheckConstraint(model.GetStereotype(CheckConstraint.DefinitionId));
            return true;
        }

        public static Schema GetSchema(this ClassModel model)
        {
            var stereotype = model.GetStereotype(Schema.DefinitionId);
            return stereotype != null ? new Schema(stereotype) : null;
        }


        public static bool HasSchema(this ClassModel model)
        {
            return model.HasStereotype(Schema.DefinitionId);
        }

        public static bool TryGetSchema(this ClassModel model, out Schema stereotype)
        {
            if (!HasSchema(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Schema(model.GetStereotype(Schema.DefinitionId));
            return true;
        }
        public static Table GetTable(this ClassModel model)
        {
            var stereotype = model.GetStereotype(Table.DefinitionId);
            return stereotype != null ? new Table(stereotype) : null;
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetTable"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        [IntentManaged(Mode.Ignore)]
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
            return model.HasStereotype(Table.DefinitionId);
        }

        public static bool TryGetTable(this ClassModel model, out Table stereotype)
        {
            if (!HasTable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Table(model.GetStereotype(Table.DefinitionId));
            return true;
        }

        public static View GetView(this ClassModel model)
        {
            var stereotype = model.GetStereotype(View.DefinitionId);
            return stereotype != null ? new View(stereotype) : null;
        }


        public static bool HasView(this ClassModel model)
        {
            return model.HasStereotype(View.DefinitionId);
        }

        public static bool TryGetView(this ClassModel model, out View stereotype)
        {
            if (!HasView(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new View(model.GetStereotype(View.DefinitionId));
            return true;
        }

        public class CheckConstraint
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8565814D-EF70-40EE-A0D3-577AB7B1254C";

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

        public class Schema
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c0f17219-ada3-47ac-80c6-7a5750cbd322";

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


        public class Table
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "dd205b32-b48b-4c77-98f5-faefb2c047ce";

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

        public class View
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6dfa2c79-4b9a-4741-9201-95a9d7631b4d";

            public View(IStereotype stereotype)
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