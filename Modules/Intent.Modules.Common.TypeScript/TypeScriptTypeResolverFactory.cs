using System.Collections.Generic;
using System.ComponentModel;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Contracts;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.Common.TypeScript
{
    [Description("Type Script Type Resolver")]
    public class TypeScriptTypeResolverFactory : ITypeResolverFactory
    {
        public string Name
        {
            get
            {
                return "TypeScript#";
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
                return new[] { "ts" } ;
            }
        }

        public ITypeResolver Create()
        {
            return new TypeScriptTypeResolver();
        }

    }
}
