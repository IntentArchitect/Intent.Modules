using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Metadata.RDBMS.Api
{
    [Obsolete("Replaced by AssociationTargetEndModelStereotypeExtensions")]
    public static class AssociationTargetEndModelExtensions
    {
        [Obsolete]
        public static ForeignKey GetForeignKey(AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Foreign Key");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }

        [Obsolete]
        public static bool HasForeignKey(AssociationTargetEndModel model)
        {
            return model.HasStereotype("Foreign Key");
        }

        [Obsolete]
        public static Index GetIndex(AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Index");
            return stereotype != null ? new Index(stereotype) : null;
        }

        [Obsolete]
        public static bool HasIndex(AssociationTargetEndModel model)
        {
            return model.HasStereotype("Index");
        }


        public class ForeignKey
        {
            private IStereotype _stereotype;

            public ForeignKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ColumnName()
            {
                return _stereotype.GetProperty<string>("Column Name");
            }

        }

        public class Index
        {
            private IStereotype _stereotype;

            public Index(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string UniqueKey()
            {
                return _stereotype.GetProperty<string>("UniqueKey");
            }

            public int? Order()
            {
                return _stereotype.GetProperty<int?>("Order");
            }

            public bool IsUnique()
            {
                return _stereotype.GetProperty<bool>("IsUnique");
            }

        }

    }
}