using System.Threading.Tasks;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Templates;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.Tests.FileBuilders.DataFileBuilder;

[UsesVerify]
public class DataFileBuilderTests
{
    [Fact]
    public async Task WithJsonWriter()
    {
        var template = new TemplateMock();

        template.DataFile = new DataFile("file")
            .WithJsonWriter()
            .WithRootObject(template, root =>
            {
                root
                    .WithValue("Field1", "Value1")
                    .WithObject("SomeDictionary", o =>
                    {
                        o.WithValue("Name", "Value");
                        o.WithArray("NestedArray", a =>
                        {
                            a.CommentedOut();
                            a.WithValue("1");
                            a.WithObject(arrayNestedDictionary => arrayNestedDictionary.WithValue("Key", "Something").WithValue("OtherKey", "Something else"));
                            a.WithValue("2");
                            a.WithArray(arrayNestedArray => arrayNestedArray.WithValue(1).WithValue(2));
                        });
                    })
                    .WithValue("AnotherValue", "The other value's contents")
                    .WithObject("AnotherDictionary", o =>
                    {
                        o.CommentedOut();
                        o.WithValue("1", "1");
                        o.WithValue("2", "2");
                    })
                    ;
            });

        template.DataFile.Build();

        await Verifier.Verify(template.DataFile.ToString());
    }

    [Fact]
    public async Task WithOclWriter()
    {
        var template = new TemplateMock();

        template.DataFile = new DataFile("file")
            .WithOclWriter()
            .WithRootObject(template, root =>
            {
                root
                    .WithValue("Field1", "Value1")
                    .WithObject("SomeDictionary", o =>
                    {
                        o.WithValue("Name", "Value");
                        o.WithArray("NestedArray", a =>
                        {
                            a.CommentedOut();
                            a.WithValue("1");
                            a.WithObject(arrayNestedDictionary => arrayNestedDictionary.WithValue("Key", "Something").WithValue("OtherKey", "Something else"));
                            a.WithValue("2");
                            a.WithArray(arrayNestedArray => arrayNestedArray.WithValue(1).WithValue(2));
                        });
                    })
                    .WithValue("AnotherValue", "The other value's contents")
                    .WithObject("AnotherDictionary", o =>
                    {
                        o.CommentedOut();
                        o.WithValue("1", "1");
                        o.WithValue("2", "2");
                    })
                    ;
            });

        template.DataFile.Build();

        await Verifier.Verify(template.DataFile.ToString());
    }

    [Fact]
    public async Task WithYamlWriter()
    {
        var template = new TemplateMock();

        template.DataFile = new DataFile("file")
            .WithYamlWriter()
            .WithRootObject(template, root =>
            {
                root
                    .WithValue("Field1", "Value1")
                    .WithObject("SomeDictionary", o =>
                    {
                        o.WithValue("Name", "Value");
                        o.WithArray("NestedArray", a =>
                        {
                            a.CommentedOut();
                            a.WithValue("1");
                            a.WithObject(arrayNestedDictionary => arrayNestedDictionary.WithValue("Key", "Something").WithValue("OtherKey", "Something else"));
                            a.WithValue("2");
                            a.WithArray(arrayNestedArray => arrayNestedArray.WithValue(1).WithValue(2));
                        });
                    })
                    .WithValue("AnotherValue", "The other value's contents")
                    .WithObject("AnotherDictionary", o =>
                    {
                        o.CommentedOut();
                        o.WithValue("1", "1");
                        o.WithValue("2", "2");
                    })
                    ;
            });

        template.DataFile.Build();

        await Verifier.Verify(template.DataFile.ToString());
    }

    [Fact(Skip = "Needs investigation")]
    public async Task CompleteYamlExample()
    {
        var template = new TemplateMock();

        template.DataFile = new DataFile("azure-pipelines")
            .WithYamlWriter(alwaysQuoteStrings: true)
            .WithRootObject(template, dictionary =>
            {
                dictionary.WithBlankLinesBetweenItems();

                dictionary.WithObject("trigger", trigger =>
                {
                    trigger.WithObject("branches", branches =>
                    {
                        branches.WithArray("include", array =>
                        {
                            array.WithValue("*");
                        });
                    });
                });

                dictionary.WithObject("pool", pool =>
                {
                    pool.WithValue("vmImage", "ubuntu-latest");
                });

                dictionary.WithArray("variables", variables =>
                {
                    variables.WithObject(variable =>
                    {
                        variable.WithValue("name", "buildConfiguration");
                        variable.WithValue("value", "debug");
                    });

                    variables.WithObject(variable =>
                    {
                        variable.WithValue("name", "intentSolutionPath");
                        variable.WithValue("value", "intent");
                    });

                    variables.WithObject(variable =>
                    {
                        variable.CommentedOut();
                        variable.WithValue("group", "Intent Architect Credentials");
                    });
                });

                dictionary.WithArray("steps", steps =>
                {
                    steps.WithBlankLinesBetweenItems();

                    steps.WithObject(step =>
                    {
                        step.WithValue("task", "DotNetCoreCLI@2");
                        step.WithValue("displayName", "dotnet build $(buildConfiguration)");
                        step.WithObject("inputs", inputs =>
                        {
                            inputs.WithValue("command", "build");
                            inputs.WithValue("projects", "**/*.csproj");
                            inputs.WithValue("arguments", "--configuration $(buildConfiguration)");
                        });
                    });

                    steps.WithObject(step =>
                    {
                        step.WithValue("task", "DotNetCoreCLI@2");
                        step.WithValue("displayName", "dotnet test");
                        step.WithObject("inputs", inputs =>
                        {
                            inputs.WithValue("command", "test");
                            inputs.WithValue("projects", "**/*Tests/*.csproj");
                            inputs.WithValue("arguments", "--configuration $(buildConfiguration)");
                        });
                    });

                    steps.WithObject(step =>
                    {
                        step.WithValue("task", "PowerShell@2");
                        step.WithValue("displayName", "install intent cli");
                        step.WithObject("inputs", inputs =>
                        {
                            inputs.WithValue("targetType", "inline");
                            inputs.WithValue("pwsh", true);
                            inputs.WithValue("script", "dotnet tool install Intent.SoftwareFactory.CLI --global");
                        });
                    });

                    steps.WithObject(step =>
                    {
                        step.WithValue("task", "PowerShell@2");
                        step.WithValue("displayName", "run intent cli");
                        step.WithObject("env", env =>
                        {
                            env.WithValue("INTENT_USER", "$(intent-architect-user)");
                            env.WithValue("INTENT_PASS", "$(intent-architect-password)");
                            env.WithValue("INTENT_SOLUTION_PATH", "$(intentSolutionPath)");
                        });
                        step.WithObject("inputs", inputs =>
                        {
                            inputs.WithValue("targetType", "inline");
                            inputs.WithValue("pwsh", true);
                            inputs.WithValue("script", """
                                                       if (($Env:INTENT_USER -Eq $null) -or ($Env:INTENT_USER -Like "`$(*")) {
                                                         Write-Host "##vso[task.logissue type=warning;]Intent Architect Credentials not configured, see https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.ContinuousIntegration.AzurePipelines/README.md#configuring-intent-architect-credentials for more information."
                                                         Return
                                                       }

                                                       intent-cli ensure-no-outstanding-changes "$Env:INTENT_USER" "$Env:INTENT_PASS" "$Env:INTENT_SOLUTION_PATH"
                                                       """);
                        });
                    });
                });
            });
        template.DataFile.Build();

        await Verifier.Verify(template.DataFile.ToString());
    }

    private class TemplateMock : IDataFileBuilderTemplate
    {
        public IDataFile DataFile { get; set; }
        public IFileMetadata GetMetadata() => throw new System.NotImplementedException();
        public bool CanRunTemplate() => throw new System.NotImplementedException();
        public string RunTemplate() => throw new System.NotImplementedException();
        public string Id { get; }
    }
}