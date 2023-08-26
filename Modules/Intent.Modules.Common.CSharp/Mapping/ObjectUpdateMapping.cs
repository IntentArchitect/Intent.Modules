﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ObjectUpdateMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ObjectUpdateMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }
    public ObjectUpdateMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetFromStatement()
    {
        if (Mapping == null) // is traversal
        {
            return GetPathText(GetFromPath(), _fromReplacements);
        }
        else
        {
            if (Children.Count == 0)
            {
                return $"{GetFromPathText()}";
            }
            else if (Model.TypeReference.IsCollection)
            {
                var from = $"UpdateHelper.CreateOrUpdateCollection({GetToPathText()}, {GetFromPathText()}, (e, d) => e.Id == d.Id, CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()})";

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");

                return from;
            }
        }

        return null;
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement()
    {
        if (Mapping == null) // is traversal
        {
          
            if (Model.TypeReference == null)
            {
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatement()))
                {
                    yield return statement.WithSemicolon();
                }
            }
            else
            {
                yield return new CSharpAssignmentStatement(GetToStatement(), $"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}({GetToStatement()}, {GetFromStatement()})");

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");
            }
        }
        else
        {
            yield return new CSharpAssignmentStatement(GetToStatement(), GetFromStatement());
        }
    }

    private void CreateUpdateMethod(string updateMethodName)
    {
        var domainTypeName = _template.GetTypeName((IElement)Model.TypeReference.Element);
        var fromField = GetFromPath().Last().Element;
        var fieldIsNullable = fromField.TypeReference.IsNullable;

        var @class = _template.CSharpFile.Classes.First();
        var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                    x.ReturnType == domainTypeName &&
                                                    x.Parameters.FirstOrDefault()?.Type == domainTypeName &&
                                                    x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)fromField.TypeReference.Element));
        if (existingMethod != null)
        {
            return;
        }
        _template.CSharpFile.AfterBuild(file =>
        {
            file.Classes.First().AddMethod(domainTypeName, updateMethodName, method =>
            {
                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                method.Private().Static();
                method.AddParameter(_template.GetTypeName(Model.TypeReference.Element.AsTypeReference(true, false)), "entity");
                method.AddParameter(_template.GetTypeName((IElement)GetFromPath().Last().Element.TypeReference.Element), "dto");

                if (fieldIsNullable)
                {
                    method.AddIfStatement("dto == null", s => s
                        .AddStatement("return null;"));
                }

                method.AddStatement($"entity ??= new {_template.GetTypeName((IElement)Model.TypeReference.Element)}();");

                SetFromReplacement(GetFromPath().Last().Element, "dto");
                SetToReplacement(GetToPath().Last().Element, "entity");
                method.AddStatements(Children.SelectMany(x => x.GetMappingStatement()).Select(x => x.WithSemicolon()));

                method.AddStatement("return entity;");
            });
        });
    }
}