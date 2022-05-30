using Intent.Engine;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.TypeScript
{
    public class TypescriptTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (ICollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, CollectionFormatter.GetOrCreate(collectionFormat));
        }

        //public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, Func<string, string> collectionFormatter)
        //{
        //    return Create(context, templateId, CollectionFormatter.GetOrCreate(collectionFormatter));
        //}

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, ICollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId).WithCollectionFormatter(collectionFormatter);
        }
    }
}
