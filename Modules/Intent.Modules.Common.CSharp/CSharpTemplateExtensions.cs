using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Templates
{
    public static class CSharpTemplateExtensions
    {
        public static string ResolveAllUsings(this ITemplate template, ISoftwareFactoryExecutionContext context, params string[] namespacesToIgnore)
        {
            var usings = template
                .GetAllTemplateDependencies()
                .SelectMany(context.FindTemplateInstances<ITemplate>)
                .Where(ti => ti != null && ti.GetMetadata().CustomMetadata.ContainsKey("Namespace"))
                .ToList()
                .Select(x => x.GetMetadata().CustomMetadata["Namespace"])
                .Union(template.GetAllDeclareUsing())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Except(namespacesToIgnore)
                .Distinct()
                .ToArray();

            foreach (var @using in usings.Where(x => x != FixUsing(x)))
            {
                Logging.Log.Warning($"When resolving usings for Template Id [{template.Id}] file [{template.GetMetadata().FileName}], " +
                                    $"a using arrived with with the format [{@using}], but should have been in the format " +
                                    $"[{FixUsing(@using)}]. The module and/or decorator author should update this module.");
            }

            usings = usings
                .Select(x => $"using {FixUsing(x)};")
                .Distinct()
                .ToArray();

            return usings.Any()
                ? usings.Aggregate((x, y) => x + Environment.NewLine + y)
                : string.Empty;
        }

        private static string FixUsing(string @using)
        {
            if (@using.StartsWith("using "))
            {
                @using = @using.Substring("using ".Length, @using.Length - "using ".Length);
            }

            if (@using.EndsWith(";"))
            {
                @using = @using.Substring(0, @using.Length - ";".Length);
            }

            return @using;
        }

        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            return template.GetAll<IDeclareUsings, string>((i) => i.DeclareUsings());
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
            template.ApplyAppSetting(field, value, null, null);
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