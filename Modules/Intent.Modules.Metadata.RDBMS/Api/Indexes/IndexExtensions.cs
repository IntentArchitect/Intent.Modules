using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    public static class IndexExtensions
    {
        public static IReadOnlyList<Index> GetIndexes(this ClassModel model)
        {
            var results = new List<Index>();

            var indexElements = new ClassExtensionModel(model.InternalElement).Indexes;

            // Modern mapped element:
            results.AddRange(indexElements
                .Select(indexModel => new Index
                {
                    IsUnique = indexModel.GetIndex().Unique(),
                    Name = indexModel.Name,
                    FilterOption = GetFilterOption(indexModel.GetIndex().Filter().AsEnum()),
                    Filter = indexModel.GetIndex().FilterCustomValue(),
                    KeyColumns = indexModel.Columns
                        .Where(x => x.GetIndexColumn().Type().AsEnum() == IndexColumnModelStereotypeExtensions.IndexColumn.TypeOptionsEnum.Key)
                        .Select(GetIndexColumn)
                        .ToArray(),
                    IncludedColumns = indexModel.Columns
                        .Where(x => x.GetIndexColumn().Type().AsEnum() == IndexColumnModelStereotypeExtensions.IndexColumn.TypeOptionsEnum.Included)
                        .Select(GetIndexColumn)
                        .ToArray()
                }));

            // Legacy stereotypes:
            results.AddRange(model.Attributes
                .Where(x => x.HasIndex())
                .GroupBy(x => x.GetIndex().UniqueKey() ?? "IX_" + model.Name + "_" + x.Name)
                .Select(index => new Index
                {
                    IsUnique = index.First().GetIndex().IsUnique(),
                    Name = index.Key,
                    FilterOption = FilterOption.Default,
                    Filter = null,
                    KeyColumns = index
                        .OrderBy(x => x.GetIndex().Order() ?? 0)
                        .Select(x => new IndexColumn
                        {
                            Name = x.Name,
                            Type = IndexColumnType.Attribute,
                            IsDerived = false,
                        })
                        .ToArray(),
                    IncludedColumns = Array.Empty<IndexColumn>()
                }));

            return results;
        }

        private static IndexColumn GetIndexColumn(IndexColumnModel model)
        {
            if (!model.InternalElement.IsMapped)
            {
                return new IndexColumn
                {
                    Name = model.Name,
                    Type = IndexColumnType.Unknown
                };
            }

            IndexColumnType type;
            switch (model.InternalElement.MappedElement?.Element.SpecializationType)
            {
                case "Attribute":
                    type = IndexColumnType.Attribute;
                    break;
                case "Association Target End":
                    type = IndexColumnType.Association;
                    break;
                default:
                    type = IndexColumnType.Unknown;
                    break;
            }

            return new IndexColumn
            {
                Name = model.Name,
                Type = type,
                IsDerived = model.InternalElement.MappedElement?.Path.Count > 1
            };
        }

        private static FilterOption GetFilterOption(IndexModelStereotypeExtensions.Index.FilterOptionsEnum filter)
        {
            switch (filter)
            {
                case IndexModelStereotypeExtensions.Index.FilterOptionsEnum.Default:
                    return FilterOption.Default;
                case IndexModelStereotypeExtensions.Index.FilterOptionsEnum.None:
                    return FilterOption.None;
                case IndexModelStereotypeExtensions.Index.FilterOptionsEnum.Custom:
                    return FilterOption.Custom;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
    }
}
