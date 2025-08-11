using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Events;

namespace Intent.Modules.Common.CSharp;

/// <summary>
/// Contains extension methods for configuring Razor configuration.
/// </summary>
public static class RazorConfigurationExtensions
{
    /// <summary>
    /// Configure additional matching options for the specified <paramref name="tagName"/> for Razor Weaver
    /// to use when determining matches of element/directives between existing and generated files.
    /// </summary>
    public static ISoftwareFactoryExecutionContext ConfigureRazorTagMatchingFor(this ISoftwareFactoryExecutionContext context, string tagName, Action<IRazorTagMatchingConfiguration> configuration)
    {
        configuration(new RazorTagMatchingConfiguration(context, tagName));
        return context;
    }

    private class RazorTagMatchingConfiguration(ISoftwareFactoryExecutionContext context, string tagName) : IRazorTagMatchingConfiguration
    {
        public IRazorTagMatchingConfiguration AllowMatchByDescendant(IReadOnlyList<string> path)
        {
            context.EventDispatcher.Publish(new AllowMatchByDescendantEvent
            {
                TagName = tagName,
                Path = path
            });

            return this;
        }

        public IRazorTagMatchingConfiguration AllowMatchByNameOnly()
        {
            context.EventDispatcher.Publish(new AllowMatchByTagNameOnlyEvent
            {
                TagName = tagName
            });

            return this;
        }

        public IRazorTagMatchingConfiguration AllowMatchByAttributes(params string[] attributeNames)
        {
            context.EventDispatcher.Publish(new AddTagNameAttributesMatchEvent
            {
                TagName = tagName,
                AttributeNames = attributeNames
            });

            return this;
        }
    }
}