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
            return @class.Attributes.Where(x => AttributeModelExtensions.HasPrimaryKey(x)).ToList();
        }

        public static string GetSurrogateKey(this ClassModel @class)
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

            return @class.GetExplicitPrimaryKey().Any() ? typeResolver.Get(@class.GetExplicitPrimaryKey().SingleOrDefault()?.Type.Element).Name : "Guid";
        }

        public static bool HasSurrogateKey(this ClassModel @class)
        {
            return @class.Attributes.Count(x => x.HasPrimaryKey() && x.Name.EndsWith("id", StringComparison.InvariantCultureIgnoreCase)) <= 1; // 0 = implicit
        }
    }
}