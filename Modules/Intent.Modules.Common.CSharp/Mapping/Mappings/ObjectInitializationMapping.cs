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

        public ClassConstructionMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
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

        public ObjectInitializationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
        {
            _template = template;
        }
        public ObjectInitializationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
        {
            _template = template;
        }

        public override CSharpStatement GetFromStatement()
        {
            if (Mapping == null)
            {
                SetToReplacement(Model, null);
                return GetConstructorStatement();
            }
            else
            {
                if (Children.Count == 0)
                {
                    return $"{GetFromPathText()}";
                }
                if (Model.TypeReference.IsCollection)
                {
                    var chain = new CSharpMethodChainStatement($"{GetFromPathText()}{(Mapping.FromPath.Last().Element.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
                    var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

                    var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
                    SetFromReplacement(GetFromPath().Last().Element, variableName);
                    SetToReplacement(GetToPath().Last().Element, null);

                    select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(GetConstructorStatement()));

                    var init = chain
                        .AddChainStatement(select)
                        .AddChainStatement("ToList()");
                    return init;
                }
                else
                {
                    return GetFromPathText();
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
                    return ctor.GetFromStatement();
                }

                var init = new CSharpObjectInitializerBlock(ctor.GetFromStatement().GetText(""));
                init.AddStatements(children.Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));
                return init;
            }
            else
            {
                var init = Model.TypeReference != null
                    ? new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}")
                    : new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model)}");
                init.AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));
                return init;
            }
        }

        public override CSharpStatement GetToStatement()
        {
            return Model.Name.ToPascalCase();
        }
    }
}
