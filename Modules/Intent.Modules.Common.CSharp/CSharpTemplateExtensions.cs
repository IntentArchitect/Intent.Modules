using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

// ReSharper disable once CheckNamespace - This is intentionally in this namespace to avoid requiring a using directive needing to be added any time it's used.
namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// Extension methods for <see cref="CSharpTemplateBase{TModel}"/>.
    /// </summary>
    public static class CSharpTemplateExtensions
    {
        /// <summary>
        /// Obsolete. Use <see cref="ResolveAllUsings(ITemplate,string[])"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string ResolveAllUsings(ITemplate template, ISoftwareFactoryExecutionContext context, params string[] namespacesToIgnore)
        {
            return ResolveAllUsings(template, namespacesToIgnore);
        }

        /// <summary>
        /// Resolves all usings for the provided <paramref name="template"/>.
        /// </summary>
        public static string ResolveAllUsings(ITemplate template, params string[] namespacesToIgnore)
        {
            var templateNamespace = GetNamespace(template) ?? string.Empty;

            var usingDirectives = template.GetAll<IDeclareUsings, string>(item => item.DeclareUsings())
                .Where(@namespace => !string.IsNullOrWhiteSpace(@namespace) && !IsRedundant(@namespace, templateNamespace))
                .Except(namespacesToIgnore)
                .Select(@namespace => $"using {@namespace};")
                .Distinct();

            return string.Join(Environment.NewLine, usingDirectives);

            static string GetNamespace(ITemplate templateInstance)
            {
                return templateInstance?.GetMetadata().CustomMetadata.TryGetValue("Namespace", out var @namespace) == true
                    ? @namespace
                    : null;
            }

            static bool IsRedundant(string otherNamespace, string templateNamespace)
            {
                return templateNamespace == otherNamespace ||
                       templateNamespace.StartsWith($"{otherNamespace}.");
            }
        }

        /// <summary>
        /// This member will be changed to be only privately accessible or possibly removed
        /// entirely, please contact Intent Architect support should you have a dependency on it.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        [FixFor_Version4("See comments in method.")]
        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            // No apparent reason this can't be private or possibly even a local method.
            return template.GetAll<IDeclareUsings, string>(i => i.DeclareUsings());
        }

        /// <summary>
        /// Adds a profile to <see href="https://docs.microsoft.com/aspnet/core/fundamentals/environments#lsj"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        public static void ApplyLaunchProfile(
            this IntentTemplateBase template,
            string profileName,
            string commandName,
            bool launchBrowser,
            string launchUrl,
            string applicationUrl)
        {
            // See below for string values:
            // https://github.com/IntentSoftware/Intent.Modules.NET/blob/e817253e96b1611142a968494a92b926307b1dbd/Modules/Intent.Modules.VisualStudio.Projects/Templates/CoreWeb/LaunchSettings/LaunchSettingsJsonTemplate.cs#L38-L44

            template.OutputTarget.ExecutionContext.EventDispatcher.Publish("LaunchProfileRegistrationEvent", new Dictionary<string, string>
            {
                ["profileName"] = profileName,
                ["commandName"] = commandName,
                ["launchBrowser"] = launchBrowser.ToString().ToLowerInvariant(),
                ["launchUrl"] = launchUrl,
                ["applicationUrl"] = applicationUrl
            });
        }

        /// <summary>
        /// Adds an item to <see href="https://docs.microsoft.com/aspnet/core/fundamentals/configuration#appsettingsjson"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        ///
        /// <param name="template">
        /// An instance of a type derived from <see cref="IntentTemplateBase"/> with an accessible
        /// instance of <see cref="IApplicationEventDispatcher"/> which is used to make the request.
        /// </param>
        ///
        /// <param name="field">
        /// The top most level field of the json file to add.
        /// </param>
        ///
        /// <param name="value">
        /// The <paramref name="value"/> to set for the <paramref name="field"/>, the provided value
        /// (including anonymous objects) will be output as a JSON structure.
        /// </param>
        [FixFor_Version4("Remove this overload so that there is a single method with optional parameters.")]
        public static void ApplyAppSetting(
            this IntentTemplateBase template,
            string field,
            object value)
        {
            template.ApplyAppSetting(field, value, null);
        }

        /// <summary>
        /// Adds an item to <see href="https://docs.microsoft.com/aspnet/core/fundamentals/configuration#appsettingsjson"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        ///
        /// <param name="template">
        /// An instance of a type derived from <see cref="IntentTemplateBase"/> with an accessible
        /// instance of <see cref="IApplicationEventDispatcher"/> which is used to make the request.
        /// </param>
        ///
        /// <param name="field">
        /// The top most level field of the json file to add.
        /// </param>
        ///
        /// <param name="value">
        /// The <paramref name="value"/> to set for the <paramref name="field"/>, the provided value
        /// (including anonymous objects) will be output as a JSON structure.
        /// </param>
        ///
        /// <param name="runtimeEnvironment">
        /// Optional. The specific appsettings.&lt;<paramref name="runtimeEnvironment"/>&gt;.json
        /// file to apply to, when not specified then the default appsettings.json file is applied
        /// to.
        /// </param>
        ///
        /// <param name="forProjectWithRole">
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// <remarks>
        /// Used for disambiguating which appsettings[.&lt;<paramref name="runtimeEnvironment"/>&gt;].json
        /// file to apply to when a solution has multiple projects each with their own
        /// appsettings[.&lt;<paramref name="runtimeEnvironment"/>&gt;].json file(s).
        /// </remarks>
        /// </param>
        [FixFor_Version4("Make the 'runtimeEnvironment' parameter have a default value of null")]
        public static void ApplyAppSetting(
            this IntentTemplateBase template,
            string field,
            object value,
            string runtimeEnvironment,
            string forProjectWithRole = null)
        {
            var request = new AppSettingRegistrationRequest(
                key: field,
                value: value,
                runtimeEnvironment: runtimeEnvironment,
                forProjectWithRole: forProjectWithRole);

            template.OutputTarget.ExecutionContext.EventDispatcher.Publish(request);

            if (!request.WasHandled)
            {
                Logging.Log.Warning($"{nameof(ApplyAppSetting)} for {nameof(field)}='{field}',{nameof(runtimeEnvironment)}='{runtimeEnvironment}',{nameof(forProjectWithRole)}='{forProjectWithRole}' " +
                                    $"was not handled. Ensure in the Visual Studio designer that you have:{Environment.NewLine}" +
                                    $" - at least one ASP.NET project{Environment.NewLine}" +
                                    (!string.IsNullOrWhiteSpace(runtimeEnvironment) ? $" - a matching 'Runtime Environment' element under the project{Environment.NewLine}" : string.Empty) +
                                    (!string.IsNullOrWhiteSpace(forProjectWithRole) ? $" - a matching 'Role' element under the project{Environment.NewLine}" : string.Empty).TrimEnd());
            }
        }

        /// <summary>
        /// Obsolete. Use <see cref="ApplyConnectionString(IntentTemplateBase,string,string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static void ApplyConnectionString(
            this IntentTemplateBase template,
            string name,
            string connectionString,
            string providerName)
        {
            template.ApplyConnectionString(name, connectionString, null, null);
        }

        /// <summary>
        /// Adds an entry to the "connectionStrings" item of <see href="https://docs.microsoft.com/aspnet/core/fundamentals/configuration#appsettingsjson"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        ///
        /// <param name="template">
        /// An instance of a type derived from <see cref="IntentTemplateBase"/> with an accessible
        /// instance of <see cref="IApplicationEventDispatcher"/> which is used to make the request.
        /// </param>
        ///
        /// <param name="name">
        /// The name of the connection string.
        /// </param>
        ///
        /// <param name="connectionString">
        /// The connection string value.
        /// </param>
        [FixFor_Version4("Remove this overload so that there is a single method with optional parameters.")]
        public static void ApplyConnectionString(
            this IntentTemplateBase template,
            string name,
            string connectionString)
        {
            template.ApplyConnectionString(name, connectionString, null, null);
        }

        /// <summary>
        /// Adds an entry to the "connectionStrings" item of <see href="https://docs.microsoft.com/aspnet/core/fundamentals/configuration#appsettingsjson"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        ///
        /// <param name="template">
        /// An instance of a type derived from <see cref="IntentTemplateBase"/> with an accessible
        /// instance of <see cref="IApplicationEventDispatcher"/> which is used to make the request.
        /// </param>
        ///
        /// <param name="name">
        /// The name of the connection string.
        /// </param>
        ///
        /// <param name="connectionString">
        /// The connection string value.
        /// </param>
        ///
        /// <param name="runtimeEnvironment">
        /// Optional. The specific appsettings.&lt;<paramref name="runtimeEnvironment"/>&gt;.json
        /// file to apply to, when not specified then the default appsettings.json file is applied
        /// to.
        /// </param>
        ///
        /// <param name="forProjectWithRole">
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// <remarks>
        /// Used for disambiguating which appsettings[.&lt;<paramref name="runtimeEnvironment"/>&gt;].json
        /// file to apply to when a solution has multiple projects each with their own
        /// appsettings[.&lt;<paramref name="runtimeEnvironment"/>&gt;].json file(s).
        /// </remarks>
        /// </param>
        [FixFor_Version4("Make the 'runtimeEnvironment' and 'forProjectWithRole' parameters have a default value of null")]
        public static void ApplyConnectionString(
            this IntentTemplateBase template,
            string name,
            string connectionString,
            string runtimeEnvironment,
            string forProjectWithRole)
        {
            var request = new ConnectionStringRegistrationRequest(
                name: name,
                connectionString: connectionString,
                providerName: null,
                runtimeEnvironment: runtimeEnvironment,
                forProjectWithRole: forProjectWithRole);

            template.OutputTarget.ExecutionContext.EventDispatcher.Publish(request);

            if (!request.WasHandled)
            {
                Logging.Log.Warning($"{nameof(ApplyConnectionString)} for {nameof(name)}='{name}',{nameof(runtimeEnvironment)}='{runtimeEnvironment}',{nameof(forProjectWithRole)}='{forProjectWithRole}' " +
                                    $"was not handled. Ensure in the Visual Studio designer that you have:{Environment.NewLine}" +
                                    $" - at least one ASP.NET project{Environment.NewLine}" +
                                    (!string.IsNullOrWhiteSpace(runtimeEnvironment) ? $" - a matching 'Runtime Environment' element under the project{Environment.NewLine}" : string.Empty) +
                                    (!string.IsNullOrWhiteSpace(forProjectWithRole) ? $" - a matching 'Role' element under the project{Environment.NewLine}" : string.Empty).TrimEnd());
            }
        }
    }
}