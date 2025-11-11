using Intent.Engine;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Dart.TypeResolvers
{
    /// <summary>
    /// Factory methods for creating instances of <see cref="ITypeSource"/>.
    /// </summary>
    public class DartTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (DartCollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, DartCollectionFormatter.Create(collectionFormat));
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, DartCollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId).WithCollectionFormatter(collectionFormatter);
        }
    }
}