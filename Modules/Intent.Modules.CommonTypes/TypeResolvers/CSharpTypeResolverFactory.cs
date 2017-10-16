using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using System.ComponentModel;

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
                return new[] { "cs" } ;
            }
        }

        public ITypeResolver Create()
        {
            return new CSharpTypeResolver();
        }

    }
}
