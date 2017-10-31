using Intent.MetaModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;

namespace Intent.SoftwareFactory.Templates
{
    public interface ITypeResolver
    {
        void AddClassTypeSource(SoftwareFactory.Templates.IClassTypeSource classTypeSource);
        string Get(ITypeReference typeInfo);
    }
}
