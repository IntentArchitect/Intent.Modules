using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Metadata.RDBMS.Api
{
    public static class ClassModelKeyExtensions
    {
        public static IList<AttributeModel> GetExplicitPrimaryKey(this ClassModel @class)
        {
            return GetSelfAndParents(@class)
                .SelectMany(s => s.Attributes)
                .Where(p => p.HasPrimaryKey())
                .ToList();
        }

        [Obsolete("Use GetSurrogateKeyName extension method")]
        public static string GetSurrogateKey(this ClassModel @class)
        {
            return GetSurrogateKeyName(@class);
        }

        public static string GetSurrogateKeyName(this ClassModel @class)
        {
            if (!HasSurrogateKey(@class))
            {
                throw new Exception($"{nameof(ClassModel)} [{@class}] does not have a surrogate key");
            }

            return @class.GetExplicitPrimaryKey().SingleOrDefault()?.Name ?? "Id";
        }

        public static string GetSurrogateKeyType(this ClassModel @class, ITypeResolver typeResolver)
        {
            if (!HasSurrogateKey(@class))
            {
                throw new Exception($"{nameof(ClassModel)} [{@class}] does not have a surrogate key");
            }

            return @class.GetExplicitPrimaryKey().Any()
                ? typeResolver.Get(@class.GetExplicitPrimaryKey().SingleOrDefault()?.Type.Element).Name
                : null;
        }

        public static bool HasSurrogateKey(this ClassModel @class)
        {
            return @class.Attributes.Count(x => x.HasPrimaryKey()) <=
                   1; // less than or equal to 1, because 0 = implicit surrogate key
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