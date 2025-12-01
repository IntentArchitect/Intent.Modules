using Intent.Engine;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Kotlin.TypeResolvers
{
    public class KotlinTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (KotlinCollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, KotlinCollectionFormatter.Create(collectionFormat));
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, KotlinCollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId).WithCollectionFormatter(collectionFormatter);
        }
    }
}
