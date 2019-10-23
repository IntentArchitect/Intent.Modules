using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class EnumLiteralModel : IEnumLiteral
    {
        private readonly ILiteral _literal;

        public EnumLiteralModel(ILiteral literal)
        {
            _literal = literal;
        }

        public string Id => _literal.Id;
        public IEnumerable<IStereotype> Stereotypes => _literal.Stereotypes;
        public string Name => _literal.Name;
        public string Value => _literal.Value;
        public string Comment => _literal.Comment;
    }
}