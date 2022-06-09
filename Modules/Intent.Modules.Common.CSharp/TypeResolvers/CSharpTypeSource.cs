using Intent.Engine;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp
{
    public class CSharpTypeSource
    {
        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return Create(context, templateId, (CSharpCollectionFormatter)null);
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat)
        {
            return Create(context, templateId, CSharpCollectionFormatter.GetOrCreate(collectionFormat));
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, CSharpCollectionFormatter collectionFormatter)
        {
            return ClassTypeSource.Create(context, templateId, CSharpCollectionFormatter.GetOrCreate).WithCollectionFormatter(collectionFormatter);
        }
    }
}
