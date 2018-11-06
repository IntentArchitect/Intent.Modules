using Intent.MetaModel.Domain;
using System;
using System.Collections.Generic;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class VisitableDomainEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Entities.VisitableDomainEntityStateDecorator";

        public override IEnumerable<string> GetInterfaces(IClass @class)
        {
            return new List<string>() { "IVisitable" };
        }

        public override IEnumerable<string> DeclareUsings()
        {
            return new List<string>() { "Intent.Framework.Core.Visitor" };
        }

        public override string BeforeProperties(IClass @class)
        {
            return @"
        void IVisitable.Accept(IVisitor v)
        {
            v.Visit(this);
        }";
        }
    }
}
