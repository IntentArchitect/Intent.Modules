using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.AppStartup;

/// <summary>
/// Abstracts working with application startup concerns as the exact statements to generate and where to place
/// them can vary depending on whether or not the use <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
/// minimal hosting model</see> and use <see href="https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements">
/// top-level statements</see> options have been selected.
/// </summary>
public interface IAppStartupFile
{
    delegate void ConfigureServiceStatement<in TStatement>(
        TStatement statement,
        IServiceConfigurationContext context) where TStatement : CSharpStatement;

    delegate void ConfigureServiceLambda(
        CSharpInvocationStatement statement,
        CSharpLambdaBlock lambdaStatement,
        IServiceConfigurationLambdaContext context);

    delegate void ConfigureAppStatement<in TStatement>(
        TStatement statement,
        IAppConfigurationContext context) where TStatement : CSharpStatement;

    delegate void ConfigureEndpointStatement<in TStatement>(
        TStatement statement,
        IEndpointConfigurationContext context) where TStatement : CSharpStatement;

    delegate void ConfigureAppLambda(
        CSharpInvocationStatement statement,
        CSharpLambdaBlock lambdaStatement,
        IAppConfigurationLambdaContext context);

    /// <summary>
    /// Returns the appropriate IHasCSharpStatements based on the combination of use top-level
    /// statements and use minimal hosting model. Can contain a mix of statements (for example,
    /// when doing minimal hosting model).
    /// </summary>
    IAppStartupFile ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure);

    /// <summary>
    /// Returns the appropriate IHasCSharpStatements based on the combination of use top-level
    /// statements and use minimal hosting model. Can contain a mix of statements (for example,
    /// when doing minimal hosting model).
    /// </summary>
    IAppStartupFile ConfigureApp(Action<IHasCSharpStatements, IAppConfigurationContext> configure);

    /// <summary>
    /// Returns the appropriate IHasCSharpStatements based on the combination of use top-level
    /// statements and use minimal hosting model. Can contain a mix of statements (for example,
    /// when doing minimal hosting model).
    /// </summary>
    IAppStartupFile ConfigureEndpoints(Action<IHasCSharpStatements, IEndpointConfigurationContext> configure);

    IAppStartupFile AddMethod(string returnType, string name, Action<IStartupMethod> configure = null, int? priority = null);

    ///// <summary>
    ///// Adds a statement which can configure services of an application. The
    ///// <see cref="IServiceConfigurationContext"/> can be used to get expressions
    ///// (such as for <c>IConfiguration</c>) which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
    ///// generic or minimal hosting model</see> is being used.</summary>
    ///// <param name="create">A factory method which should return a statement.</param>
    ///// <param name="configure">Additional configuration of the statement.</param>
    IAppStartupFile AddServiceConfiguration(
        Func<IServiceConfigurationContext, CSharpStatement> create,
        ConfigureServiceStatement<CSharpStatement> configure = null,
        int? priority = null)
            => AddServiceConfiguration<CSharpStatement>(create, configure, priority);

    IAppStartupFile AddServiceConfiguration<TStatement>(
        Func<IServiceConfigurationContext, TStatement> create,
        ConfigureServiceStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement;

    IAppStartupFile AddServiceConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda configure = null,
        int? priority = null);

    IAppStartupFile AddContainerRegistration(
        Func<IServiceConfigurationContext, CSharpStatement> create,
        ConfigureServiceStatement<CSharpStatement> configure = null,
        int? priority = null)
            => AddContainerRegistration<CSharpStatement>(create, configure);

    IAppStartupFile AddContainerRegistration<TStatement>(
        Func<IServiceConfigurationContext, TStatement> create,
        ConfigureServiceStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement;

    IAppStartupFile AddContainerRegistrationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda configure = null,
        int? priority = null);

    IAppStartupFile AddAppConfiguration(
        Func<IAppConfigurationContext, CSharpStatement> create,
        ConfigureAppStatement<CSharpStatement> configure = null,
        int? priority = null)
            => AddAppConfiguration<CSharpStatement>(create, configure);

    IAppStartupFile AddAppConfiguration<TStatement>(
        Func<IAppConfigurationContext, TStatement> create,
        ConfigureAppStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement;

    IAppStartupFile AddAppConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureAppLambda configure = null,
        int? priority = null);

    IAppStartupFile AddUseEndpointsStatement(
        Func<IEndpointConfigurationContext, CSharpStatement> create,
        ConfigureEndpointStatement<CSharpStatement> configure = null,
        int? priority = null)
            => AddUseEndpointsStatement<CSharpStatement>(create, configure, priority);

    IAppStartupFile AddUseEndpointsStatement<TStatement>(
        Func<IEndpointConfigurationContext, TStatement> create,
        ConfigureEndpointStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement;

    public interface IAppConfigurationLambdaContext : ILambdaConfigurationContext, IAppConfigurationContext { }

    public interface IServiceConfigurationLambdaContext : ILambdaConfigurationContext, IServiceConfigurationContext { }

    public interface ILambdaConfigurationContext
    {
        IReadOnlyList<string> Parameters { get; }
    }
    public interface IEndpointConfigurationContext : IAppConfigurationContext
    {
        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.routing.iendpointroutebuilder">IConfiguration</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string Endpoints { get; }
    }

    public interface IAppConfigurationContext
    {
        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.extensions.configuration.iconfiguration">IConfiguration</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.builder.iapplicationbuilder">IApplicationBuilder</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string App { get; }

        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment">IWebHostEnvironment</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string Env { get; }
    }

    public interface IServiceConfigurationContext
    {
        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.extensions.configuration.iconfiguration">IConfiguration</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// The expression to access an instance of <see href="https://learn.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection">IServiceCollection</see>
        /// which can vary depending on whether <see href="https://learn.microsoft.com/aspnet/core/migration/50-to-60#new-hosting-model">
        /// generic or minimal hosting model</see> is being used.
        /// </summary>
        string Services { get; }
    }
}