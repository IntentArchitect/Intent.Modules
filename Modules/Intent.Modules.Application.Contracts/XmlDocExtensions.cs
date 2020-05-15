using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Contracts
{
    public static class XmlDocExtensions
    {
        // TODO: GCB - this probably shouldn't be supported as a default Intent feature
        internal static IEnumerable<string> GetXmlDocLines(this IHasStereotypes hasStereotypes)
        {
            var text = hasStereotypes.GetStereotypeProperty<string>("XmlDoc", "Content");

            return string.IsNullOrWhiteSpace(text)
                ? new string[0]
                : text
                    .Replace("\r\n", "\r")
                    .Replace("\n", "\r")
                    .Split('\r');
        }
    }
}