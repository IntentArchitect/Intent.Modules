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
        /// <summary>
        /// Adds a default <see cref="IClassTypeSource"/> that is used when resolving type names of classes.
        /// </summary>
        /// <param name="classTypeSource"></param>
        void AddClassTypeSource(SoftwareFactory.Templates.IClassTypeSource classTypeSource);

        /// <summary>
        /// Adds an <see cref="IClassTypeSource"/> that is only used to resolve type names when <see cref="Get(Intent.MetaModel.Common.ITypeReference,string)"/> is called for the specific <see cref="contextName"/>.
        /// </summary>
        /// <param name="classTypeSource"></param>
        /// <param name="contextName"></param>
        void AddClassTypeSource(SoftwareFactory.Templates.IClassTypeSource classTypeSource, string contextName);
        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/> using <see cref="IClassTypeSource"/> that have been added for the specified <see cref="contextName"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="contextName"></param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo, string contextName);
    }
}
