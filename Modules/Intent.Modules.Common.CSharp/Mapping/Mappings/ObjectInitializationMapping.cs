using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping
{
    public class ObjectInitializationMapping : CSharpMappingBase
    {
        private readonly MappingModel _mappingModel;
        private readonly ICSharpFileBuilderTemplate _template;

        public ObjectInitializationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        public override CSharpStatement GetSourceStatement()
        {
            if (Model.TypeReference == null)
            {
                SetTargetReplacement(Model, null);
                return GetConstructorStatement();
            }
            else
            {
                if (Children.Count == 0)
                {
                    return $"{GetSourcePathText()}";
                }
                if (Model.TypeReference.IsCollection)
                {
                    Template.CSharpFile.AddUsing("System.Linq");
                    var chain = new CSharpMethodChainStatement($"{GetSourcePathText()}{(Mapping.SourceElement.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
                    var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

                    var variableName = GetVariableNameForSelect();
                    SetSourceReplacement(GetSourcePath().Last().Element, variableName);
                    SetTargetReplacement(GetTargetPath().Last().Element, null);

                    select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(GetConstructorStatement()));

                    var init = chain
                        .AddChainStatement(select)
                        .AddChainStatement("ToList()");
                    return init;
                }
                else
                {
                    if (Mapping != null)
                    {
                        return GetSourcePathText();
                    }
                    else
                    {
                        // TODO: add ternary check to mappings for when the source path could be nullable.
                        SetTargetReplacement(GetTargetPath().Last().Element, null);
                        return GetConstructorStatement();
                    }
                }
            }
        }

        private ConstructorMapping FindConstructorMappingInHierarchy(IList<ICSharpMapping> childMappings)
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

        private IList<ICSharpMapping> FindPropertyMappingsInHierarchy(IList<ICSharpMapping> childMappings)
        {
            if (!childMappings.Any())
            {
                return new List<ICSharpMapping>();
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
                if (!children.Any())
                {
                    // use constructor only:
                    return ctorMapping.GetSourceStatement();
                }

                // use constructor and object initialization syntax:
                var hybridInit = new CSharpObjectInitializerBlock(ctorMapping.GetSourceStatement()); 
                hybridInit.AddStatements(children.Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
                return hybridInit;
            }

            var fileBuilderCtors = GetFileBuilderConstructors();
            if (fileBuilderCtors.Any())
            {
                var targetCtor = fileBuilderCtors.First();
                var ctorInit = new CSharpInvocationStatement($"new {_template.GetTypeName(targetCtor.Element)}").WithoutSemicolon();
                var childMappings = FindPropertyMappingsInHierarchy(Children).ToList();
                foreach (var ctorParameter in targetCtor.Ctor.Parameters)
                {
                    var match = childMappings.FirstOrDefault(child => ctorParameter.TryGetReferenceForModel(child.Mapping.TargetElement, out var match) && match.Name == ctorParameter.Name);
                    ctorInit.AddArgument(match?.GetSourceStatement() ?? "default");
                }
                return ctorInit;
            }
            
            var propInit = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
                ? new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}")
                : new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model)}");
            foreach (var child in Children)
            {
                propInit.AddStatements(child.GetMappingStatements());
            }
            return propInit;
        }

        private IReadOnlyList<(CSharpConstructor Ctor, IElement Element)> GetFileBuilderConstructors()
        {
            var returnTypeElement = ((IElement)_mappingModel.Model)?.TypeReference?.Element;
            if (returnTypeElement is null)
            {
                return ArraySegment<(CSharpConstructor, IElement)>.Empty;
            }

            if (_template.GetTypeInfo(returnTypeElement.AsTypeReference())?.Template is not ICSharpFileBuilderTemplate template)
            {
                return ArraySegment<(CSharpConstructor, IElement)>.Empty;
            }

            var constructors = template.CSharpFile.TypeDeclarations.SelectMany(s => s.Constructors).ToArray();
            var mapTargetElements = FindPropertyMappingsInHierarchy(Children).Select(s => s.Mapping.TargetElement).ToList();

            return constructors
                .Where(ctor => mapTargetElements
                    .All(target => ctor.Parameters.Any(param => param.TryGetReferenceForModel(target, out var match) && param.Name == match.Name)))
                .Select(s => (Ctor: s, MetadataElement: (IElement)returnTypeElement))
                .ToList();
        }

        private string GetVariableNameForSelect()
        {
            var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
            if (string.IsNullOrEmpty(variableName))
            {
                variableName = Char.ToLower(Model.Name[0]).ToString();
            }

            return variableName;
        }

        public override CSharpStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToPascalCase();
        }
    }
}
