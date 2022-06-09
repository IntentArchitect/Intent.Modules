using Intent.Engine;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    public class JavaTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (JavaCollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, JavaCollectionFormatter.GetOrCreate(collectionFormat));
        }

        //public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, Func<string, string> collectionFormatter)
        //{
        //    return Create(context, templateId, new CollectionFormatter(collectionFormatter));
        //}

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, JavaCollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId, JavaCollectionFormatter.GetOrCreate).WithCollectionFormatter(collectionFormatter);
        }
    }
}
