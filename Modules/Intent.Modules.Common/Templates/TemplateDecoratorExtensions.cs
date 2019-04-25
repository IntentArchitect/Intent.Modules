using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public static class TemplateDecoratorExtensions
    {
        public static string Aggregate<TDecorator>(this IEnumerable<TDecorator> decorators, Func<TDecorator, string> property)
            where TDecorator : ITemplateDecorator
        {
            var decoratorValues = decorators.Select(property).Where(x => !string.IsNullOrEmpty(x)).ToList();
            return decoratorValues.Any() ? decoratorValues.Aggregate((x, y) => x + (y.StartsWith("\r\n") ? y : "\r\n" + y)) : "";
        }

        public static string Aggregate<TDecorator>(this IEnumerable<TDecorator> decorators, Func<TDecorator, string[]> property)
            where TDecorator : ITemplateDecorator
        {
            var decoratorValues = decorators.SelectMany(property).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return decoratorValues.Any() ? decoratorValues.Aggregate((x, y) => x + (y.StartsWith("\r\n") ? y : "\r\n" + y)) : "";
        }
    }
}
