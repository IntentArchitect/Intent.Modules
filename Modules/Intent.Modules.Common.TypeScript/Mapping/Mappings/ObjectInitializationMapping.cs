#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Angular.Mapping
{
    /// <inheritdoc />
    public class ObjectInitializationMapping : TypescriptMappingBase
    {
        private readonly MappingModel _mappingModel;
        private readonly ITypescriptTemplate _template;

        /// <inheritdoc />
        public ObjectInitializationMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        /// <inheritdoc />
        [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
        public ObjectInitializationMapping(MappingModel model, ITypescriptFileBuilderTemplate template) : this(model, (ITypescriptTemplate)template)
        {
        }

        /// <inheritdoc />
        public override TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = null)
        {
            //if (Model.TypeReference == null)
            //{
            //    SetTargetReplacement(Model, null);
            //    return GetConstructorStatement();
            //}

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

        private TypescriptStatement GetConstructorStatement()
        {
            //var ctorMapping = FindConstructorMappingInHierarchy(Children);
            //if (ctorMapping != null)
            //{
            //    var children = FindPropertyMappingsInHierarchy(Children).ToList();
            //    if (children.Count == 0)
            //    {
            //        // use constructor only:
            //        return ctorMapping.GetSourceStatement();
            //    }

            //    // use constructor and object initialization syntax:
            //    var hybridInit = new CSharpObjectInitializerBlock(ctorMapping.GetSourceStatement());
            //    hybridInit.AddStatements(children.Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
            //    return hybridInit;
            //}

            //var fileBuilderConstructors = GetFileBuilderConstructors();
            //if (fileBuilderConstructors.Any())
            //{
            //    var targetCtor = fileBuilderConstructors[0];
            //    var ctorInit = new TypescriptStatement($"new {_template.GetTypeName(targetCtor.Element)}");
            //    var childMappings = FindPropertyMappingsInHierarchy(Children).ToList();
            //    foreach (var ctorParameter in targetCtor.Ctor.Parameters)
            //    {
            //        var match = childMappings.FirstOrDefault(child =>
            //            ctorParameter.TryGetReferenceForModel(child.Mapping.TargetElement, out var match) && match.Name == ctorParameter.Name);
            //        ctorInit.AddArgument(match?.GetSourceStatement() ?? "default");
            //    }

            //    if (childMappings.Count == targetCtor.Ctor.Parameters.Count)
            //    {
            //        return ctorInit;
            //    }

            //    // use constructor and object initialization syntax:
            //    var hybridInit = new CSharpObjectInitializerBlock(ctorInit);
            //    hybridInit.AddStatements(childMappings
            //        .Where(x => !targetCtor.Ctor.Parameters.Any(ctorParameter => ctorParameter.TryGetReferenceForModel(x.Mapping.TargetElement, out var match) && match.Name == ctorParameter.Name))
            //        .Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
            //    return hybridInit;
            //}

            var objectType = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null ?
                _template.GetTypeName(Model.TypeReference.AsContructableType()) :
                _template.GetTypeName((IElement)Model);

            var variableName = Model.SpecializationType.ToCamelCase();

            var sb = new StringBuilder();

            sb.AppendLine($"const {variableName}: {objectType} = {{");

            foreach (var child in Children)
            {
                var statements = child.GetMappingStatements();

                foreach(var statement in statements)    
                {
                    sb.AppendLine($"        {statement.GetText(string.Empty)},");
                }
            }

            // remove the last , char
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    if (sb[i] == ',')
                    {
                        sb.Remove(i, 1);
                    }
                    break;
                }
            }

            sb.AppendLine($"    }}");


            //var propInit = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
            //    ? new TypescriptStatement($"new {_template.GetTypeName(Model.TypeReference.AsContructableType())}")
            //    : new TypescriptStatement($"new {_template.GetTypeName((IElement)Model)}");
            //foreach (var child in Children)
            //{
            //    propInit.AddStatements(child.GetMappingStatements());
            //}

            return sb.ToString();
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

        private static (int maxDepth, ITypescriptMapping? deepestChild) GetMaxDepthWithChild(ITypescriptMapping mapping, HashSet<ITypescriptMapping> childMappingsSet)
        {
            if (!mapping.Children.Any())
                return (0, childMappingsSet.Contains(mapping) ? mapping : null);

            var deepest = mapping.Children
                .Select(child => GetMaxDepthWithChild(child, childMappingsSet)) // Recursively get depth for each child
                .MaxBy(result => result.maxDepth); // Find the child with the max depth

            var validChild = deepest.deepestChild ?? (childMappingsSet.Contains(mapping) ? mapping : null);

            return (deepest.maxDepth + 1, validChild);
        }

        private IReadOnlyList<(TypescriptMethod Ctor, IElement Element)> GetFileBuilderConstructors()
        {
            var returnTypeElement = ((IElement)_mappingModel.Model)?.TypeReference?.Element;
            if (returnTypeElement is null ||
                _template.GetTypeInfo(returnTypeElement.AsTypeReference())?.Template is not ITypescriptFileBuilderTemplate template)
            {
                return ArraySegment<(TypescriptMethod, IElement)>.Empty;
            }

            var constructors = template.TypescriptFile.Classes.SelectMany(s => s.Methods.Where(m => m.Name == "ngOnInit")).ToArray();
            var mapTargetElements = FindPropertyMappingsInHierarchy(Children).Where(s => s.Mapping != null).Select(s => s.Mapping.TargetElement).ToList();

            return constructors
                .Where(ctor => mapTargetElements
                    .All(target => ctor.Parameters.Any(param => param.TryGetReferenceForModel(target, out var match) && param.Name == match.Name)))
                .Select(s => (Ctor: s, MetadataElement: (IElement)returnTypeElement))
                .ToList();
        }


        private static ITypescriptMapping? FindConstructorMappingInHierarchy(IList<ITypescriptMapping> childMappings)
        {
            // TODO
            //var ctor = childMappings.SingleOrDefault(x => x is ConstructorMapping && x.Model.TypeReference == null);
            var ctor = childMappings.SingleOrDefault(x => x.Model.Name == "OnInit" && x.Model.TypeReference == null);
            if (ctor != null)
            {
                return ctor;
            }

            ctor = childMappings.SingleOrDefault(IsMappingStaticConstructor);
            if (ctor != null)
            {
                return ctor;
            }

            foreach (var childrenMapping in childMappings.OfType<MapChildrenMapping>())
            {
                ctor = FindConstructorMappingInHierarchy(childrenMapping.Children);
                if (ctor != null)
                {
                    return ctor;
                }
            }

            return null;

            static bool IsMappingStaticConstructor(ITypescriptMapping mapping)
            {
                if (mapping is not StaticMethodInvocationMapping)
                {
                    return false;
                }
                
                // It maps to the operation, not the Entity
                var entityId = ((IElement)mapping.Model).ParentId;
                var operationReturnTypeRef = mapping.Model.TypeReference;

                // Check for generics
                var genericParams = operationReturnTypeRef.GenericTypeParameters.ToList();
                if (genericParams.Count == 1 && genericParams[0].ElementId == entityId)
                {
                    // return true if we want to support this result-pattern scenario in the future
                    throw new NotSupportedException("Wrapping the entity in a generic type is not supported for static constructor creation mappings.");
                }

                return operationReturnTypeRef?.ElementId == entityId;
            }
        }

        private static List<ITypescriptMapping> FindPropertyMappingsInHierarchy(IList<ITypescriptMapping> childMappings)
        {
            if (!childMappings.Any())
            {
                return [];
            }

            // TODO
            var results = childMappings.Where(x => x.Model.TypeReference != null).ToList();
            //var results = childMappings.Where(x => (x is not ConstructorMapping and not MapChildrenMapping and not StaticMethodInvocationMapping) && x.Model.TypeReference != null).ToList();
            results.AddRange(FindPropertyMappingsInHierarchy(childMappings.OfType<MapChildrenMapping>().SelectMany(x => x.Children).ToList()));
            return results;
        }

        /// <inheritdoc />
        public override TypescriptStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToCamelCase();
        }
    }
}