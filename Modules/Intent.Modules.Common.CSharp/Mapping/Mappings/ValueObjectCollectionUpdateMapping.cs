using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.Common.CSharp.Mapping
{
    public class ValueObjectCollectionUpdateMapping : CSharpMappingBase
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private readonly MappingModel _mappingModel;

        public ValueObjectCollectionUpdateMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
        {
            _mappingModel = model; 
            _template = template;
        }

        public override IEnumerable<CSharpStatement> GetMappingStatements()
        {
            yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
        }

        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
        {
            if (Children.Count == 0)
            {
                return $"{GetSourcePathText()}";
            }
            else if (Model.TypeReference.IsCollection)
            {
                var from = $"{_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({GetTargetPathText()}, {GetSourcePathText()}, (e, d) => e.Equals({GetConstructorStatement("d").GetText("            ")}), CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()})";

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");

                return from;
            }
            return null;
        }

        private CSharpStatement GetConstructorStatement(string variableName)
        {
            var ctor = new ConstructorMapping(_mappingModel.GetCollectionItemModel(), _template);
            ctor.SetSourceReplacement(GetSourcePath().Last().Element, variableName);
            var result = ctor.GetSourceStatement();
            return result;
        }

        private void CreateUpdateMethod(string updateMethodName)
        {
            var domainModel = (IElement)Model.TypeReference.Element;
            var implementationName = _template.GetTypeName(domainModel);

            var fromField = GetSourcePath().Last().Element;
            var fieldIsNullable = fromField.TypeReference.IsNullable;
            var fromFieldNullable = fieldIsNullable ? "?" : string.Empty;
            string nullableChar = _template.OutputTarget.GetProject().NullableEnabled ? "?" : "";

            var @class = _template.CSharpFile.Classes.First();
            var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                        x.ReturnType == implementationName &&
                                                        x.Parameters.FirstOrDefault()?.Type == implementationName &&
                                                        x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)fromField.TypeReference.Element));
            if (existingMethod != null)
            {
                return;
            }
            _template.CSharpFile.AfterBuild(file =>
            {
                file.Classes.First().AddMethod($"{implementationName}{fromFieldNullable}", updateMethodName, method =>
                {
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                    method.Private().Static();
                    method.AddParameter($"{implementationName}{nullableChar}", "valueObject");
                    method.AddParameter(_template.GetTypeName((IElement)GetSourcePath().Last().Element.TypeReference.Element), "dto");
                    method.AddIfStatement("valueObject is null", stmt => 
                    {
                        stmt.AddStatement($"return {GetConstructorStatement("dto").GetText("                ")};");
                    });
                    method.AddStatement("return valueObject;");
                });
            });
        }
    }
}