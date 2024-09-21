#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IHtmlElement : IRazorFileNodeBase<IHtmlElement>
{
    string Name { get; set; }
    string? Text { get; set; }
    IHtmlElement AddAttribute(string name, string? value = null);
    IHtmlElement AddClass(string className);
    IHtmlElement AddAttributeIfNotEmpty(string name, string? value);
    IHtmlElement SetAttribute(string name, string? value = null);
    IHtmlAttribute? GetAttribute(string name);
    bool HasAttribute(string name, string? value = null);
    IHtmlElement WithText(string text);
    void Remove();
}