
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using System.ComponentModel;
using Intent.Modules.CommonTypes.Contracts;
using Intent.SoftwareFactory.Templates;

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

        public int Priotiry
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
