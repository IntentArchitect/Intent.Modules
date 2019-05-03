using Intent.Modules.Common.Types.Contracts;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
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
