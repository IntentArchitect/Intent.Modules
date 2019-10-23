using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public class DomainMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public DomainMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IEnumerable<IClass> GetClasses()
        {
            var cache = new Dictionary<string, Class>();
            var classes = _metadataManager.GetMetadata<Metadata.Models.IElement>("Domain").Where(x => x.SpecializationType == "Class").ToList();
            var result = classes.Select(x => cache.ContainsKey(x.Id) ? cache[x.Id] : new Class(x, cache)).ToList();
            return result;
        }

        public IEnumerable<IClass> GetClasses(string applicationId)
        {
            return GetClasses().Where(x => x.Application.Id == applicationId);
        }

        public IEnumerable<IEnum> GetEnums()
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Domain").Where(x => x.SpecializationType == "Enum").ToList();
            return types.Select(x => new EnumModel(x)).ToList();
        }

        public IEnumerable<IEnum> GetEnums(string applicationId)
        {
            return GetEnums().Where(x => x.Application.Id == applicationId);
        }

        public IEnumerable<ITypeDefinition> GetTypeDefinitions()
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Domain").Where(x => x.SpecializationType == "Type-Definition").ToList();
            return types.Select(x => new TypeDefinition(x)).ToList();
        }

        public IEnumerable<ITypeDefinition> GetTypeDefinitions(string applicationId)
        {
            return GetTypeDefinitions().Where(x => x.Application.Id == applicationId);
        }

    }
}