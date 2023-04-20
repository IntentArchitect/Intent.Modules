using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Metadata.DocumentDB.Api.Extensions
{
    public static class ClassModelKeyExtensions
    {
        public static IList<AttributeModel> GetPrimaryKeys(this ClassModel @class)
        {
            return GetSelfAndParents(@class)
                .SelectMany(s => s.Attributes)
                .Where(p => p.HasPrimaryKey())
                .ToList();
        }

        private static IEnumerable<ClassModel> GetSelfAndParents(ClassModel @classModel)
        {
            yield return @classModel;
            var parent = @classModel.ParentClass;
            while (parent != null)
            {
                yield return parent;
                parent = parent.ParentClass;
            }
        }
    }
}