using Intent.Engine;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    public class JavaTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (ICollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, JavaCollectionFormatter.GetOrCreate(collectionFormat));
        }

        //public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, Func<string, string> collectionFormatter)
        //{
        //    return Create(context, templateId, new CollectionFormatter(collectionFormatter));
        //}

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, ICollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId).WithCollectionFormatter(collectionFormatter);
        }
    }
}
