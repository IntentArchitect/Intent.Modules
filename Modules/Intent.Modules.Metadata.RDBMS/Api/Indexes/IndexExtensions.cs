using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    /// <summary>
    /// Extension methods for getting <see cref="Index">Indexes</see> from a class.
    /// </summary>
    public static class IndexExtensions
    {
        /// <summary>
        /// Retrieves the all the <see cref="Index"/> entries for a <see cref="ClassModel"/>.
        /// </summary>
        public static IReadOnlyList<Index> GetIndexes(this ClassModel model)
        {
            var results = new List<Index>();

            var indexElements = new ClassExtensionModel(model.InternalElement).Indexes;

            // Modern mapped element:
            results.AddRange(indexElements
                .Select(indexModel => new Index
                {
                    IsUnique = indexModel.GetSettings().Unique(),
                    Name = indexModel.Name,
                    UseDefaultName = indexModel.GetSettings().UseDefaultName(),
                    FilterOption = GetFilterOption(indexModel.GetSettings().Filter().AsEnum()),
                    Filter = indexModel.GetSettings().FilterCustomValue(),
                    KeyColumns = indexModel.Columns
                        .Where(x => x.GetSettings().Type().AsEnum() == IndexColumnModelStereotypeExtensions.Settings.TypeOptionsEnum.Key)
                        .Select(GetIndexColumn)
                        .ToArray(),
                    IncludedColumns = indexModel.Columns
                        .Where(x => x.GetSettings().Type().AsEnum() == IndexColumnModelStereotypeExtensions.Settings.TypeOptionsEnum.Included)
                        .Select(GetIndexColumn)
                        .ToArray(),
                    FillFactor = indexModel.GetSettings().FillFactor()
                }));

            results.AddRange(model.Attributes
                .Where(x => x.HasIndex() && string.IsNullOrWhiteSpace(x.GetIndex().UniqueKey()))
                .Select(index => new Index
                {
                    IsUnique = index.GetIndex().IsUnique(),
                    Name = string.Empty,
                    UseDefaultName = true,
                    FilterOption = FilterOption.Default,
                    Filter = null,
                    KeyColumns = new[] { GetIndexColumn(index) },
                    IncludedColumns = Array.Empty<IndexColumn>()
                }));

            // Legacy stereotypes (where Unique Key used):
            results.AddRange(model.Attributes
                .Where(x => x.HasIndex() && !string.IsNullOrWhiteSpace(x.GetIndex().UniqueKey()))
                .GroupBy(x => x.GetIndex().UniqueKey())
                .Select(index => new Index
                {
                    IsUnique = index.First().GetIndex().IsUnique(),
                    Name = index.Key,
                    UseDefaultName = string.IsNullOrWhiteSpace(index.Key),
                    FilterOption = FilterOption.Default,
                    Filter = null,
                    KeyColumns = index
                        .OrderBy(x => x.GetIndex().Order() ?? 0)
                        .Select(x => GetIndexColumn(x))
                        .ToArray(),
                    IncludedColumns = Array.Empty<IndexColumn>()
                }));

            return results;
        }

        private static IndexColumn GetIndexColumn(AttributeModel model)
        {
            return new IndexColumn
            {
                Name = model.Name,
                SortDirection = GetSortDirection(model.GetIndex().SortDirection().AsEnum()),
                SourceType = model.InternalElement
            };
        }


        private static IndexColumn GetIndexColumn(IndexColumnModel model)
        {
            return new IndexColumn
            {
                Name = model.Name,
                SortDirection = GetSortDirection(model.GetSettings().SortDirection().AsEnum()),
                SourceType = model.InternalElement.MappedElement?.Element,
            };
        }

        private static SortDirection GetSortDirection(IndexColumnModelStereotypeExtensions.Settings.SortDirectionOptionsEnum sortDirection)
        {
            switch (sortDirection)
            {
                case IndexColumnModelStereotypeExtensions.Settings.SortDirectionOptionsEnum.Ascending:
                    return SortDirection.Ascending;
                case IndexColumnModelStereotypeExtensions.Settings.SortDirectionOptionsEnum.Descending:
                    return SortDirection.Descending;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
            }
        }

        private static SortDirection GetSortDirection(AttributeModelStereotypeExtensions.Index.SortDirectionOptionsEnum sortDirection)
        {
            switch (sortDirection)
            {
                case AttributeModelStereotypeExtensions.Index.SortDirectionOptionsEnum.Ascending:
                    return SortDirection.Ascending;
                case AttributeModelStereotypeExtensions.Index.SortDirectionOptionsEnum.Descending:
                    return SortDirection.Descending;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
            }
        }

        private static FilterOption GetFilterOption(IndexModelStereotypeExtensions.Settings.FilterOptionsEnum filter)
        {
            switch (filter)
            {
                case IndexModelStereotypeExtensions.Settings.FilterOptionsEnum.Default:
                    return FilterOption.Default;
                case IndexModelStereotypeExtensions.Settings.FilterOptionsEnum.None:
                    return FilterOption.None;
                case IndexModelStereotypeExtensions.Settings.FilterOptionsEnum.Custom:
                    return FilterOption.Custom;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
    }
}
