using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
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
        /// <para>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </para>
        ///
        /// <para>
        /// Any kind of value can be used for <paramref name="value"/>, including an anonymous object, and it will be output as a JSON structure.
        /// </para>
        /// </remarks>
        public static void ApplyAppSetting(
            this IntentTemplateBase template,
            string key,
            object value)
        {
            template.OutputTarget.ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(key, value));
        }

        /// <summary>
        /// Adds an entry to the "connectionStrings" item of <see href="https://docs.microsoft.com/aspnet/core/fundamentals/configuration#appsettingsjson"/>
        /// if it does not already exist.
        /// </summary>
        /// <remarks>
        /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
        /// </remarks>
        public static void ApplyConnectionString(
            this IntentTemplateBase template,
            string name,
            string connectionString,
            string providerName)
        {
            template.OutputTarget.ExecutionContext.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(name, connectionString, providerName));
        }
    }
}