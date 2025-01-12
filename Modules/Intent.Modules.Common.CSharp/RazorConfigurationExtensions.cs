using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Events;

namespace Intent.Modules.Common.CSharp;

/// <summary>
/// Contains extension methods for configuring Razor file management.
/// </summary>
public static class RazorConfigurationExtensions
{
    /// <summary>
    /// Allows configuring Razor.
    /// </summary>
    public static ISoftwareFactoryExecutionContext ConfigureRazor(this ISoftwareFactoryExecutionContext context, Action<IRazorConfigurator> configure)
    {
        configure(new RazorConfigurator(context));
        return context;
    }

    private class RazorConfigurator(ISoftwareFactoryExecutionContext context) : IRazorConfigurator
    {
        public IRazorConfigurator AllowMatchByTagNameOnly(string tagName)
        {
            context.EventDispatcher.Publish(new AllowMatchByTagNameOnlyEvent
            {
                TagName = tagName
            });

            return this;
        }

        public IRazorConfigurator AddTagNameAttributeMatch(string tagName, string attributeName)
        {

            context.EventDispatcher.Publish(new AddTagNameAttributeMatchEvent
            {
                TagName = tagName,
                AttributeName = attributeName
            });

            return this;
        }
    }
}