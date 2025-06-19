#nullable enable
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers
{
    internal class CSharpDeclarationWrapper<TImpl, TSpecialization>(CSharpDeclaration<TImpl> wrapped) :
        CSharpMetadataBaseWrapper(wrapped),
        ICSharpDeclaration<TSpecialization>
        where TImpl : CSharpDeclaration<TImpl>
        where TSpecialization : class, ICSharpDeclaration<TSpecialization>
    {
        public IEnumerable<ICSharpAttribute> Attributes { get; }

        public TSpecialization AddAttribute(string name, Action<ICSharpAttribute>? configure = null)
        {
            wrapped.AddAttribute(name, configure);
            return this as TSpecialization ?? throw new InvalidOperationException();
        }

        public TSpecialization AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute>? configure = null)
        {
            wrapped.AddAttribute((CSharpAttribute)attribute, configure);
            return this as TSpecialization ?? throw new InvalidOperationException();
        }

        public TSpecialization WithComments(string xmlComments)
        {
            wrapped.WithComments(xmlComments);
            return this as TSpecialization ?? throw new InvalidOperationException();
        }

        public TSpecialization WithComments(IEnumerable<string> xmlComments)
        {
            wrapped.WithComments(xmlComments);
            return this as TSpecialization ?? throw new InvalidOperationException();
        }
    }
}
