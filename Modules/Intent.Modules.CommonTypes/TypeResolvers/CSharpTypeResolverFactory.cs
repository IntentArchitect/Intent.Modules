using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.TypeResolvers;
using Intent.Modules.CommonTypes.Contracts;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    [Description("C# Type Resolver")]
    public class CSharpTypeResolverFactory : ITypeResolverFactory
    {
        public string Name
        {
            get
            {
                return "C#";
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
                return new[] { "cs" } ;
            }
        }

        public ITypeResolver Create()
        {
            return new CSharpTypeResolver();
        }

    }
}
