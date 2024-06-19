#nullable enable
namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IHtmlElement : IRazorFileNodeBase<IHtmlElement>
{
    string Name { get; set; }
    string? Text { get; set; }
    IHtmlElement AddAttribute(string name, string value = null);
    IHtmlElement AddClass(string className);
    IHtmlElement AddAttributeIfNotEmpty(string name, string value);
    IHtmlElement SetAttribute(string name, string value = null);
    IHtmlAttribute? GetAttribute(string name);
    bool HasAttribute(string name, string value = null);
    IHtmlElement WithText(string text);
    IHtmlElement AddAbove(IRazorFileNode node);
    IHtmlElement AddAbove(params IRazorFileNode[] nodes);
    IHtmlElement AddBelow(IRazorFileNode node);
    IHtmlElement AddBelow(params IRazorFileNode[] nodes);
    void Remove();
}