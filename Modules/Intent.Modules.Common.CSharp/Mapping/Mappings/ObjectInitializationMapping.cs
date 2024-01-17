using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

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

                    var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
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
            //var ctor = Children.SingleOrDefault(x => x is ConstructorMapping && x.Model.TypeReference == null);
            var ctor = FindConstructorMappingInHierarchy(Children);
            if (ctor != null)
            {
                var children = FindPropertyMappingsInHierarchy(Children).ToList();
                if (!children.Any())
                {
                    // use constructor only:
                    return ctor.GetSourceStatement();
                }

                // use constructor and object initialization syntax:
                var init = new CSharpObjectInitializerBlock(ctor.GetSourceStatement()); 
                init.AddStatements(children.Select(x => new CSharpAssignmentStatement(x.GetTargetStatement(), x.GetSourceStatement())));
                return init;
            }
            else
            {
                var init = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
                    ? new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}")
                    : new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model)}");
                foreach (var child in Children)
                {
                    init.AddStatements(child.GetMappingStatements());
                }
                //init.AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));
                return init;
            }
        }

        public override CSharpStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToPascalCase();
        }
    }
}
