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
    public static class AssociationTargetEndModelStereotypeExtensions
    {
        public static ForeignKey GetForeignKey(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("dfe17723-99ee-4554-9be3-f4c90dd48078");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetForeignKey"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        [IntentManaged(Mode.Ignore)]
        public static IReadOnlyCollection<ForeignKey> GetForeignKeys(this AssociationTargetEndModel model)
        {
            var stereotypes = model
                .GetStereotypes("Foreign Key")
                .Select(stereotype => new ForeignKey(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasForeignKey(this AssociationTargetEndModel model)
        {
            return model.HasStereotype("dfe17723-99ee-4554-9be3-f4c90dd48078");
        }

        public static bool TryGetForeignKey(this AssociationTargetEndModel model, out ForeignKey stereotype)
        {
            if (!HasForeignKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ForeignKey(model.GetStereotype("dfe17723-99ee-4554-9be3-f4c90dd48078"));
            return true;
        }

        public static Index GetIndex(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("bbe43b90-c20d-4fdb-8a55-9037a5f6bd0b");
            return stereotype != null ? new Index(stereotype) : null;
        }

        public static IReadOnlyCollection<Index> GetIndices(this AssociationTargetEndModel model)
        {
            var stereotypes = model
                .GetStereotypes("bbe43b90-c20d-4fdb-8a55-9037a5f6bd0b")
                .Select(stereotype => new Index(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasIndex(this AssociationTargetEndModel model)
        {
            return model.HasStereotype("bbe43b90-c20d-4fdb-8a55-9037a5f6bd0b");
        }

        public static bool TryGetIndex(this AssociationTargetEndModel model, out Index stereotype)
        {
            if (!HasIndex(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Index(model.GetStereotype("bbe43b90-c20d-4fdb-8a55-9037a5f6bd0b"));
            return true;
        }

        public static JoinTable GetJoinTable(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("5679fb86-e403-4dc0-bf25-8446ef2d1d03");
            return stereotype != null ? new JoinTable(stereotype) : null;
        }


        public static bool HasJoinTable(this AssociationTargetEndModel model)
        {
            return model.HasStereotype("5679fb86-e403-4dc0-bf25-8446ef2d1d03");
        }

        public static bool TryGetJoinTable(this AssociationTargetEndModel model, out JoinTable stereotype)
        {
            if (!HasJoinTable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new JoinTable(model.GetStereotype("5679fb86-e403-4dc0-bf25-8446ef2d1d03"));
            return true;
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

            public SortDirectionOptions SortDirection()
            {
                return new SortDirectionOptions(_stereotype.GetProperty<string>("Sort Direction"));
            }

            public class SortDirectionOptions
            {
                public readonly string Value;

                public SortDirectionOptions(string value)
                {
                    Value = value;
                }

                public SortDirectionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Ascending":
                            return SortDirectionOptionsEnum.Ascending;
                        case "Descending":
                            return SortDirectionOptionsEnum.Descending;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAscending()
                {
                    return Value == "Ascending";
                }
                public bool IsDescending()
                {
                    return Value == "Descending";
                }
            }

            public enum SortDirectionOptionsEnum
            {
                Ascending,
                Descending
            }

        }

        public class JoinTable
        {
            private IStereotype _stereotype;

            public JoinTable(IStereotype stereotype)
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