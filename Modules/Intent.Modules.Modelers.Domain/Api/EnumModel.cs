using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class EnumModel : IEnum
    {
        private readonly IElement _element;

        public EnumModel(IElement element)
        {
            _element = element;
            Folder = Api.Folder.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_element.ParentElement) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _element.Name;
        public IElementApplication Application => _element.Application;
        public IList<IEnumLiteral> Literals => _element.Literals.Select(x => new EnumLiteralModel(x)).ToList<IEnumLiteral>();
        public string Comment => _element.Comment;
    }
}