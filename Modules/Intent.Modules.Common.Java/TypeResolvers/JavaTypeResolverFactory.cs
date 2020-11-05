using System.Collections.Generic;
using System.ComponentModel;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Contracts;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    [Description("Java Type Resolver")]
    public class JavaTypeResolverFactory : ITypeResolverFactory
    {
        public string Name
        {
            get
            {
                return "Java";
            }
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public IEnumerable<string> SupportedFileTypes
        {
            get
            {
                return new[] { "java" };
            }
        }

        public ITypeResolver Create()
        {
            return new JavaTypeResolver();
        }

    }
}
