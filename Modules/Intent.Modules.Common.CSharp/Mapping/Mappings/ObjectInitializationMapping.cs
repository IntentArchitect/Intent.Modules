using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping
{
    public class ClassConstructionMapping : CSharpMappingBase
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public ClassConstructionMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
        {
            _template = template;
        }

        public ClassConstructionMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
        {
            _template = template;
        }
    }

    public class ObjectInitializationMapping : CSharpMappingBase
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public ObjectInitializationMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
        {
            _template = template;
        }
        public ObjectInitializationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
        {
            _template = template;
        }

        public override CSharpStatement GetSourceStatement()
        {
            if (Mapping == null)
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
                    return GetSourcePathText();
                }
            }
        }

        private CSharpStatement GetConstructorStatement()
        {
            var ctor = Children.SingleOrDefault(x => x is ImplicitConstructorMapping && x.Model.TypeReference == null);
            if (ctor != null)
            {
                var children = Children.Where(x => x is not ImplicitConstructorMapping || x.Model.TypeReference != null).ToList();
                if (!children.Any())
                {
                    return ctor.GetSourceStatement();
                }

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
            return Model.Name.ToPascalCase();
        }
    }
}
