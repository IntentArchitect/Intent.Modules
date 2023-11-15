using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.AppStartup;

public interface IProgramFile
{
    delegate void HostBuilderChainStatementConfiguration(CSharpLambdaBlock lambdaBlock, IReadOnlyList<string> parameters);

    //IProgramFile AddHostBuilderLambda(string methodName, CSharpLambdaBlock lambda);

    IProgramFile ConfigureAppConfiguration(bool requiresContext, HostBuilderChainStatementConfiguration configure = null, int priority = 0) =>
        ConfigureHostBuilderChainStatement("ConfigureAppConfiguration", requiresContext ? new[] { "context", "config" } : new[] { "config" }, configure, priority);

    IProgramFile ConfigureAppLogging(bool requiresContext, HostBuilderChainStatementConfiguration configure = null, int priority = 0) =>
        ConfigureHostBuilderChainStatement("ConfigureLogging", requiresContext ? new[] { "context", "logBuilder" } : new[] { "logBuilder" }, configure, priority);

    IProgramFile ConfigureHostBuilderChainStatement(string methodName, IEnumerable<string> parameters, HostBuilderChainStatementConfiguration configure = null, int priority = 0);

    /// <remarks>
    /// Can only be used when <see cref="UsesMinimalHostingModel"/> is <see langword="true"/>.
    /// </remarks>>
    IProgramFile AddHostBuilderConfigurationStatement(string statement, Action<CSharpStatement> configure = null, int priority = 0) =>
        AddHostBuilderConfigurationStatement<CSharpStatement>(statement, configure);

    /// <remarks>
    /// Can only be used when <see cref="UsesMinimalHostingModel"/> is <see langword="true"/>.
    /// </remarks>>
    IProgramFile AddHostBuilderConfigurationStatement<TStatement>(TStatement statement, Action<TStatement> configure = null, int priority = 0)
        where TStatement : CSharpStatement;

    IProgramFile ConfigureMainStatementsBlock(Action<IHasCSharpStatements> configure);

    IProgramFile AddMethod(string returnType, string name, Action<IStartupMethod> configure = null, int priority = 0);
    bool UsesMinimalHostingModel { get; }
}