using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
{
    public interface IRequireTypeResolver
    {

        ITypeResolver Types { get; set; }
    }
}
