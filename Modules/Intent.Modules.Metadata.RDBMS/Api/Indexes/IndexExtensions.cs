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
                        .ToArray()
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
                    KeyColumns = new[] {
                        new IndexColumn
                        {
                            Name = index.Name,
                            SourceType = index.InternalElement
                        }
                    },
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
                        .Select(x => new IndexColumn
                        {
                            Name = x.Name,
                            SourceType = x.InternalElement
                        })
                        .ToArray(),
                    IncludedColumns = Array.Empty<IndexColumn>()
                }));

            return results;
        }

        private static IndexColumn GetIndexColumn(IndexColumnModel model)
        {
            return new IndexColumn
            {
                Name = model.Name,
                SourceType = model.InternalElement.MappedElement?.Element,
            };
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
