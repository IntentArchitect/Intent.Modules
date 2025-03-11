﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ObjectUpdateMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ObjectUpdateMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }
    public ObjectUpdateMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        if (Mapping == null) // is traversal
        {

            if (Model.TypeReference == null)
            {
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatements()))
                {
                    yield return statement.WithSemicolon();
                }
            }
            else if (Model is IAssociationEnd associationEnd
                     && !associationEnd.OtherEnd().TypeReference.IsCollection
                     && !associationEnd.OtherEnd().TypeReference.IsNullable)
            {
                if (associationEnd.TypeReference.IsNullable)
                {
                    yield return $"{GetTargetPathText()} ??= new {_template.GetTypeName((IElement)associationEnd.TypeReference.Element)}();";
                }
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatements()))
                {
                    yield return statement.WithSemicolon();
                }
            }
            else
            {
                yield return new CSharpAssignmentStatement(GetTargetStatement(), $"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}({GetTargetStatement()}, {GetSourceStatement()})");

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");
            }
        }
        else
        {
            yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
        }
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        if (Mapping == null) // is traversal
        {
            return GetSourcePathText();
        }
        else
        {
            if (Children.Count == 0)
            {
                return $"{GetSourcePathText()}";
            }
            else if (Model.TypeReference.IsCollection)
            {
                var from = $"{_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({GetTargetPathText()}, {GetSourcePathText()}, (e, d) => {GetPrimaryKeyComparisonMappings()}, CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()})";

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");

                return from;
            }
        }

        return null;
    }

    private string GetPrimaryKeyComparisonMappings()
    {
        // get all elements on the target element (which is a primary key)
        var qualifyingMappings = Children.Select(c => c.Mapping).Where(m => m.TargetElement.HasStereotype("Primary Key"));

        // return a default if there are no primary keys.
        if (!qualifyingMappings.Any())
        {
            return "true";
        }

        return string.Join(" && ", qualifyingMappings.Select(m => $"e.{m.TargetElement?.Name ?? "Id"} == d.{m.SourceElement?.Name ?? "Id"}"));
    }

    //ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
    private static bool CreateEntityInterfaces(IApplicationSettingsProvider provider) => bool.TryParse(provider.GetGroup("c4d1e35c-7c0d-4926-afe0-18f17563ce17").GetSetting("0456dafe-a46e-466b-bf23-1fb35c094899")?.Value.ToPascalCase(), out var result) && result;

    private void CreateUpdateMethod(string updateMethodName)
    {
        var domainModel = (IElement)Model.TypeReference.Element;
        var createEntityInterfaces = CreateEntityInterfaces(_template.ExecutionContext.Settings);
        var implementationName = _template.GetTypeName("Domain.Entity.Primary"/*TemplateRoles.Domain.Entity.EntityImplementation*/, domainModel);
        var interfaceName = createEntityInterfaces ? _template.GetTypeName("Domain.Entity.Interface"/*TemplateRoles.Domain.Entity.Interface*/, domainModel) : implementationName;

        var fromField = GetSourcePath().Last().Element;
        var fieldIsNullable = fromField.TypeReference.IsNullable;
        var fromFieldNullable = fieldIsNullable ? "?" : string.Empty;
        string nullableChar = _template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
       
        _template.CSharpFile.AfterBuild(file =>
        {
            var @class = file.Classes.FirstOrDefault(c => c.HasMetadata("handler")) ?? file.Classes.First();

            var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                            x.ReturnType == interfaceName &&
                            (x.Parameters.FirstOrDefault()?.Type == interfaceName || x.Parameters.FirstOrDefault()?.Type == $"{interfaceName}{nullableChar}") && 
                            x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)fromField.TypeReference.Element));

            if (existingMethod != null)
            {
                return;
            }
            
            @class.AddMethod($"{interfaceName}{fromFieldNullable}", updateMethodName, method =>
            {
                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                method.Private().Static();
                method.AddParameter($"{interfaceName}{nullableChar}", "entity");
                method.AddParameter(_template.GetTypeName((IElement)GetSourcePath().Last().Element.TypeReference.Element), "dto");

                if (fieldIsNullable)
                {
                    method.AddIfStatement("dto == null", s => s
                        .AddStatement("return null;"));
                }

                method.AddStatement($"entity ??= new {implementationName}();");

                SetSourceReplacement(GetSourcePath().Last().Element, "dto");
                SetTargetReplacement(GetTargetPath().Last().Element, "entity");
                method.AddStatements(Children.SelectMany(x => x.GetMappingStatements()).Select(x => x.WithSemicolon()));

                method.AddStatement("return entity;");
            });
        });
    }
}