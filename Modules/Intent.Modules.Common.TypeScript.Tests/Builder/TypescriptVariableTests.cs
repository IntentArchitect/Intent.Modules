using Intent.Modules.Common.TypeScript.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests.Builder;

public class TypescriptVariableTests
{
    [Fact]
    public async Task EmptyObjectVariable()
    {
        var fileBuilder = new TypescriptFile("", null)
            .AddVariable("variableOne", v =>
            {
                v.Export().Const();
                v.WithObjectValue();
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task PopulatedObjectVariable()
    {
        var fileBuilder = new TypescriptFile("", null)
            .AddVariable("routes", v =>
            {
                v.Const().Export();
                v.WithObjectValue(o =>
                {
                    o.AddField("field1", "\"one\"");
                    o.AddField("field2", "\"two\"");
                    o.AddField("field3", "\"three\"");
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task ArrayWithObjectVariable()
    {
        var fileBuilder = new TypescriptFile("", null)
            .AddVariable("routes", v =>
            {
                v.Const().Export();
                v.WithArrayValue(a =>
                {
                    a.AddObject(o =>
                    {
                        o.AddField("field1", "\"one\"");
                        o.AddField("field2", "\"two\"");
                        o.AddField("field3", "\"three\"");
                    });
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task ObjectWithArrayVariableNamed()
    {
        var fileBuilder = new TypescriptFile("", null);
        fileBuilder.SetIndentation(2);

        fileBuilder.AddVariable("appConfig", "ApplicationConfig", v =>
            {
                v.Const().Export();
                v.WithObjectValue(obj =>
                {
                    var providersArray = new TypescriptVariableArray();
                    providersArray.Indentation = fileBuilder.Indentation;
                    providersArray.AddValue("methodCallOne()");
                    providersArray.AddValue("methodCallOne()");

                    obj.AddField("providers", providersArray);
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task ArrayWithObjectNoFieldSeperatorVariable()
    {
        var fileBuilder = new TypescriptFile("", null)
            .AddVariable("routes", v =>
            {
                v.Const().Export();
                v.WithArrayValue(a =>
                {
                    a.AddObject(o =>
                    {
                        o.WithFieldsSeparated(TypescriptCodeSeparatorType.None);

                        o.AddField("field1", "\"one\"");
                        o.AddField("field2", "\"two\"");
                        o.AddField("field3", "\"three\"");
                    });

                    a.AddObject(o =>
                    {
                        o.WithFieldsSeparated(TypescriptCodeSeparatorType.None);

                        o.AddField("field4", "\"four\"");
                        o.AddField("field5", "\"five\"");
                        o.AddField("field6", "\"six\"");
                    });
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task ExpressionFunctionVariable()
    {
        var fileBuilder = new TypescriptFile("", null);
        fileBuilder.AddVariable("decoratorSample", v =>
            {
                v.WithExpressionFunctionValue(exp =>
                {
                    exp.AddParameter("identifier?", "string");

                    var innerExp = new TypeScriptVariableExpressionFunction
                    {
                        Indentation = fileBuilder.Indentation
                    };
                    innerExp.AddParameter("target", "any")
                        .AddParameter("propertyKey?", "string")
                        .AddParameter("descriptor?", "any");

                    exp.AddStatement($"return {innerExp.GetText("")}");
                });

                v.Export().Const();
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task DecoaratorObjectVariable()
    {
        var fileBuilder = new TypescriptFile("", null);
        fileBuilder.SetIndentation(2);

        fileBuilder.AddClass("MyClass", c =>
        {
            c.AddDecorator("Component", component =>
            {
                var obj = new TypescriptVariableObject
                {
                    Indentation = fileBuilder.Indentation
                };
                obj.AddField("selector", $"'selectorValue'");
                obj.AddField("standalone", "true");
                obj.AddField("templateUrl", $"'templateUrlValue'");
                obj.AddField("styleUrls", $"['styleUrlsValue']");

                component.AddArgument(obj.GetText(""));
            });
        })
        .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }
}
