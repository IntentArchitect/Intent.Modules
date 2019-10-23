using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class TypeDefinition : ITypeDefinition
    {
        private readonly IElement _element;

        public TypeDefinition(IElement element)
        {
            _element = element;
            Folder = Api.Folder.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_element.ParentElement) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _element.Name;
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);
        public IElementApplication Application => _element.Application;
        public string Comment => _element.Comment;
    }
}