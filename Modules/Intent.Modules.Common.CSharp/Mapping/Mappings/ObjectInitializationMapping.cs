#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping
{
    /// <inheritdoc />
    public class ObjectInitializationMapping : CSharpMappingBase
    {
        private readonly MappingModel _mappingModel;
        private readonly ICSharpTemplate _template;

        /// <inheritdoc />
        public ObjectInitializationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        /// <inheritdoc />
        [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
        public ObjectInitializationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template)
        {
        }

        /// <inheritdoc />
        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
        {
            if (Model.TypeReference == null)
            {
                SetTargetReplacement(Model, null);
                return GetConstructorStatement();
            }

            if (Children.Count == 0)
            {
                return $"{GetSourcePathText()}";
            }

            if (Model.TypeReference.IsCollection)
            {
                var m = new SelectToListMapping(_mappingModel, _template)
                {
                    Parent = Parent
                };

                return m.GetSourceStatement(Model.TypeReference.IsNullable);
            }

            // TODO: add ternary check to mappings for when the source path could be nullable.
            var lastTargetPathElement = GetTargetPath().Last().Element;
            SetTargetReplacement(lastTargetPathElement, null); // Needed for inheritance mappings - path element to be removed from invocation path
            if (lastTargetPathElement.TypeReference.Element is not null)
            {
                SetTargetReplacement(lastTargetPathElement.TypeReference.Element, null); // Same as above but for parameter types
            }

            return GetConstructorStatement();
        }

        private static ConstructorMapping? FindConstructorMappingInHierarchy(IList<ICSharpMapping> childMappings)
        {
            var ctor = childMappings.SingleOrDefault(x => x is ConstructorMapping && x.Model.TypeReference == null);
            if (ctor != null)
            {
                return (ConstructorMapping)ctor;
            }

            foreach (var childrenMapping in childMappings.OfType<MapChildrenMapping>())
            {
                ctor = FindConstructorMappingInHierarchy(childrenMapping.Children);
                if (ctor != null)
                {
                    return (ConstructorMapping)ctor;
                }
            }

            return null;
        }

        private static List<ICSharpMapping> FindPropertyMappingsInHierarchy(IList<ICSharpMapping> childMappings)
        {
            if (!childMappings.Any())
            {
                return [];
            }

            var results = childMappings.Where(x => (x is not ConstructorMapping and not MapChildrenMapping) && x.Model.TypeReference != null).ToList();
            results.AddRange(FindPropertyMappingsInHierarchy(childMappings.OfType<MapChildrenMapping>().SelectMany(x => x.Children).ToList()));
            return results;
        }

        private CSharpStatement GetConstructorStatement()
        {
            var ctorMapping = FindConstructorMappingInHierarchy(Children);
            if (ctorMapping != null)
            {
                var children = FindPropertyMappingsInHierarchy(Children).ToList();
                if (children.Count == 0)
                {
                    // use constructor only:
                    return ctorMapping.GetSourceStatement();
                }

                // use constructor and object initialization syntax:
                var hybridInit = new CSharpObjectInitializerBlock(ctorMapping.GetSourceStatement());
                hybridInit.AddStatements(children.Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
                return hybridInit;
            }

            var fileBuilderConstructors = GetFileBuilderConstructors();
            if (fileBuilderConstructors.Any())
            {
                var targetCtor = fileBuilderConstructors[0];
                var ctorInit = new CSharpInvocationStatement($"new {_template.GetTypeName(targetCtor.Element)}").WithoutSemicolon();
                var childMappings = FindPropertyMappingsInHierarchy(Children).ToList();
                foreach (var ctorParameter in targetCtor.Ctor.Parameters)
                {
                    var match = childMappings.FirstOrDefault(child =>
                        ctorParameter.TryGetReferenceForModel(child.Mapping.TargetElement, out var match) && match.Name == ctorParameter.Name);
                    ctorInit.AddArgument(match?.GetSourceStatement() ?? "default");
                }

                if (childMappings.Count == targetCtor.Ctor.Parameters.Count)
                {
                    return ctorInit;
                }

                // use constructor and object initialization syntax:
                var hybridInit = new CSharpObjectInitializerBlock(ctorInit);
                hybridInit.AddStatements(childMappings
                    .Where(x => !targetCtor.Ctor.Parameters.Any(ctorParameter => ctorParameter.TryGetReferenceForModel(x.Mapping.TargetElement, out var match) && match.Name == ctorParameter.Name))
                    .Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
                return hybridInit;
            }

            var propInit = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
                ? new CSharpObjectInitializerBlock($"new {_template.GetTypeName(Model.TypeReference.AsContructableType())}")
                : new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model)}");
            foreach (var child in Children)
            {
                propInit.AddStatements(child.GetMappingStatements());
            }

            return GetNullableAwareInstantiation(Model, Children, propInit);
        }

        /// <inheritdoc />
        protected override IList<IElementMappingPathTarget> GetSourcePath()
        {
            if (Mapping != null)
            {
                return Mapping.SourcePath;
            }

            // get all mappings
            var childMappings = GetAllChildren().Where(c => c.Mapping != null).ToList();
            // get the depth from the end hierarchy of children and get the lowest child as well
            var (pathDepth, deepestChild) = GetMaxDepthWithChild(this, [.. childMappings]);

            // get the mapping based off the lowest child.
            // when we calculate the depth from the bottom, we want to make sure the "bottom" is the same child mapping
            // so we calculate it from the lowest item
            var mapping = childMappings.FirstOrDefault(c => c == deepestChild)?.Mapping ?? childMappings.First().Mapping;

            // if this has a mix of object init statements (mapping is null) and normal mapping, then adjust the depth by 1
            // this is to cater for scenarios where there is a hierarchy of objects without any mappings inside them
            //pathDepth = pathDepth - (Children.Any(x => x.Mapping != null) && Children.Any(x => x.Mapping == null) ||
            //            // adjust the depth for collections
            //            (mapping.SourcePath.ToList().Any(s => s.Element.TypeReference.IsCollection)) ? 1 : 0);

            var toPath = mapping.SourcePath.ToList();

            return toPath.Take(toPath.Count - pathDepth).ToList();
        }

        private static (int maxDepth, ICSharpMapping? deepestChild) GetMaxDepthWithChild(ICSharpMapping mapping, HashSet<ICSharpMapping> childMappingsSet)
        {
            if (!mapping.Children.Any())
                return (0, childMappingsSet.Contains(mapping) ? mapping : null);

            var deepest = mapping.Children
                .Select(child => GetMaxDepthWithChild(child, childMappingsSet)) // Recursively get depth for each child
                .MaxBy(result => result.maxDepth); // Find the child with the max depth

            var validChild = deepest.deepestChild ?? (childMappingsSet.Contains(mapping) ? mapping : null);

            return (deepest.maxDepth + 1, validChild);
        }

        private IReadOnlyList<(CSharpConstructor Ctor, IElement Element)> GetFileBuilderConstructors()
        {
            var returnTypeElement = ((IElement)_mappingModel.Model)?.TypeReference?.Element;
            if (returnTypeElement is null ||
                _template.GetTypeInfo(returnTypeElement.AsTypeReference())?.Template is not ICSharpFileBuilderTemplate template)
            {
                return ArraySegment<(CSharpConstructor, IElement)>.Empty;
            }

            var constructors = template.CSharpFile.TypeDeclarations.SelectMany(s => s.Constructors).ToArray();
            var mapTargetElements = FindPropertyMappingsInHierarchy(Children).Where(s => s.Mapping != null).Select(s => s.Mapping.TargetElement).ToList();

            return constructors
                .Where(ctor => mapTargetElements
                    .All(target => ctor.Parameters.Any(param => param.TryGetReferenceForModel(target, out var match) && param.Name == match.Name)))
                .Select(s => (Ctor: s, MetadataElement: (IElement)returnTypeElement))
                .ToList();
        }

        /// <inheritdoc />
        public override CSharpStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToPascalCase();
        }
    }
}