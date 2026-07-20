#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.AppStartup
{
    using static IProgramFile;

    public class ProgramFile : IProgramFile
    {
        private readonly CSharpTemplateBase<object> _template;
        private readonly CSharpFile _cSharpFile;
        private readonly bool _usesTopLevelStatements;

        public ProgramFile(
            CSharpTemplateBase<object> template,
            bool usesTopLevelStatements,
            bool usesMinimalHostingModel)
        {
            if (template is not ICSharpFileBuilderTemplate fileBuilderTemplate)
            {
                throw new Exception($"template must implement {nameof(ICSharpFileBuilderTemplate)}");
            }

            _template = template;
            _cSharpFile = fileBuilderTemplate.CSharpFile;
            _usesTopLevelStatements = usesTopLevelStatements;
            UsesMinimalHostingModel = usesMinimalHostingModel;
        }

        public bool UsesMinimalHostingModel { get; }

        public IProgramFile ConfigureHostBuilderChainStatement(
            string methodName,
            IEnumerable<string> parameters,
            HostBuilderChainStatementConfiguration? configure = null,
            int priority = 0)
        {
            var parametersAsArray = parameters.ToArray();

            if (!UsesMinimalHostingModel)
            {
                var hostBuilder = (IHasCSharpStatements?)_cSharpFile.TopLevelStatements?.FindMethod("CreateHostBuilder") ??
                                  _cSharpFile.Classes.First().FindMethod("CreateHostBuilder");

                var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder!.Statements.First();
                var appConfigurationBlock = (CSharpInvocationStatement)hostBuilderChain
                    .FindStatement(stmt => stmt.ToString()!.StartsWith(methodName));

                var lambda = EnsureWeHaveEditableLambdaBlock(appConfigurationBlock);

                if (appConfigurationBlock == null)
                {
                    lambda = new EditableCSharpLambdaBlock("()");

                    appConfigurationBlock = new CSharpInvocationStatement(methodName)
                        .WithoutSemicolon()
                        .AddArgument(lambda);
                    hostBuilderChain.Statements.Last()
                        .InsertAbove(appConfigurationBlock);
                }

                SetParameters(lambda, parametersAsArray);
                configure?.Invoke(lambda, (IReadOnlyList<string>)lambda.Metadata["parameters"]);
            }
            else
            {
                var startupTemplate = (IAppStartupTemplate)_template.ExecutionContext.FindTemplateInstance(IAppStartupTemplate.RoleName);

                startupTemplate.StartupFile.ConfigureServices((targetBlock, _) =>
                {
                    var statementsToCheck = new List<CSharpStatement>();
                    ExtractPossibleStatements(targetBlock, statementsToCheck);
                    var appConfigurationBlock = statementsToCheck.OfType<CSharpInvocationStatement>().FirstOrDefault(x => x.ToString()!.TrimStart().StartsWith($"builder.Host.{methodName}("));

                    var lambda = EnsureWeHaveEditableLambdaBlock(appConfigurationBlock);

                    if (appConfigurationBlock == null)
                    {
                        lambda = new EditableCSharpLambdaBlock("()");

                        var statementToInsertBelow = targetBlock.Statements.LastOrDefault(x => x.HasMetadata("is-host-builder-configuration-statement")) ??
                                                     targetBlock.FindStatement(x => x.HasMetadata("is-builder-statement"));

                        statementToInsertBelow.InsertBelow(new CSharpInvocationStatement($"builder.Host.{methodName}")
                            .AddArgument(lambda)
                            .SeparatedFromPrevious()
                            .AddMetadata("is-host-builder-configuration-statement", true));
                    }

                    SetParameters(lambda, parametersAsArray);
                    configure?.Invoke(lambda, (IReadOnlyList<string>)lambda.Metadata["parameters"]);
                });
            }

            return this;

            static void SetParameters(EditableCSharpLambdaBlock lambda, IReadOnlyList<string> requestedParameters)
            {
                if (!lambda.TryGetMetadata<List<string>>("parameters", out var parameters))
                {
                    parameters = new List<string>();
                    lambda.AddMetadata("parameters", parameters);
                }

                if (parameters.Count >= requestedParameters.Count)
                {
                    return;
                }

                parameters.AddRange(requestedParameters.Skip(parameters.Count));

                lambda.UpdateText(parameters.Count == 1
                    ? parameters[0]
                    : $"({string.Join(", ", parameters)})");
            }

            static void ExtractPossibleStatements(IHasCSharpStatements targetBlock, List<CSharpStatement> statementsToCheck)
            {
                foreach (var statement in targetBlock.Statements)
                {
                    if (statement is CSharpInvocationStatement)
                    {
                        statementsToCheck.Add(statement);
                    }
                    else if (statement is IHasCSharpStatements container)
                    {
                        foreach (var nested in container.Statements)
                        {
                            statementsToCheck.Add(nested);
                        }
                    }
                }
            }

            static EditableCSharpLambdaBlock EnsureWeHaveEditableLambdaBlock(CSharpInvocationStatement appConfigurationBlock)
            {
                EditableCSharpLambdaBlock? lambda;
                var configBlockStatement = appConfigurationBlock?.Statements.First();
                if (configBlockStatement is CSharpLambdaBlock lambdaConfig and not EditableCSharpLambdaBlock)
                {
                    lambda = EditableCSharpLambdaBlock.CreateFrom(lambdaConfig);
                    appConfigurationBlock!.Statements.RemoveAt(0);
                    appConfigurationBlock.AddArgument(lambda);
                }
                else
                {
                    lambda = (EditableCSharpLambdaBlock?)appConfigurationBlock?.Statements.First();
                }

                return lambda;
            }
        }

        public IProgramFile AddHostBuilderConfigurationStatement<TStatement>(
            TStatement statement,
            Action<TStatement>? configure = null,
            int priority = 0) where TStatement : CSharpStatement
        {
            if (!UsesMinimalHostingModel)
            {
                throw new InvalidOperationException("Only allowed when using minimal hosting model");
            }

            _cSharpFile.OnBuild(file =>
            {
                var targetBlock = _usesTopLevelStatements
                    ? (IHasCSharpStatements)file.TopLevelStatements
                    : file.Classes.First().Methods.First(x => x.Name == "Main");

                var statementToInsertBelow = targetBlock.Statements.LastOrDefault(x => x.HasMetadata("is-host-builder-configuration-statement")) ??
                                             targetBlock.FindStatement(x => x.HasMetadata("is-builder-statement"));

                statementToInsertBelow.InsertBelow(statement);
                statement.AddMetadata("is-host-builder-configuration-statement", true);
                configure?.Invoke(statement);
            });

            return this;
        }

        public IProgramFile ConfigureMainStatementsBlock(Action<IHasCSharpStatements> configure)
        {
            _cSharpFile.OnBuild(file =>
            {
                var targetBlock = _usesTopLevelStatements
                    ? (IHasCSharpStatements)file.TopLevelStatements
                    : file.Classes.First().Methods.First(x => x.Name == "Main");

                configure(targetBlock);
            });

            return this;
        }

        public IProgramFile AddMethod(
            string returnType,
            string name,
            Action<IStartupMethod>? configure = null,
            int priority = 0)
        {
            StartupMethod.CreateOn(
                template: _template,
                returnType: returnType,
                name: name,
                usesTopLevelStatements: _usesTopLevelStatements,
                configure: configure);
            return this;
        }

        private class EditableCSharpLambdaBlock : CSharpLambdaBlock
        {
            public EditableCSharpLambdaBlock(string invocation) : base(invocation) { }

            public void UpdateText(string text) => Text = text;

            public static EditableCSharpLambdaBlock CreateFrom(CSharpLambdaBlock original)
            {
                var update = new EditableCSharpLambdaBlock(original.Text);
                if (original.HasExpressionBody)
                {
                    update.WithExpressionBody(original.Statements.First());
                }
                else
                {
                    foreach (var statement in original.Statements)
                    {
                        update.Statements.Add(statement);
                    }
                }

                return update;
            }
        }
    }
}
