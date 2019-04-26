
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.TypeResolvers;
using Intent.Modules.CommonTypes.Contracts;
using Intent.Templates;

namespace Intent.Modules.CommonTypes.TypeResolvers
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
