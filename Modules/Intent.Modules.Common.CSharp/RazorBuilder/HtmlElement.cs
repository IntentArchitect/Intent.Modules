using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class HtmlElement : RazorFileNodeBase<HtmlElement>, IRazorFileNode
{
    public string Name { get; set; }
    public string Text { get; set; }
    public Dictionary<string, HtmlAttribute> Attributes { get; set; } = new Dictionary<string, HtmlAttribute>();

    public HtmlElement(string name, RazorFile file) : base(file)
    {
        Name = name;
    }

    public HtmlElement AddAttribute(string name, string value = null)
    {
        Attributes.Add(name, new HtmlAttribute(name, value));
        return this;
    }

    public HtmlElement AddClass(string className)
    {
        if (!Attributes.TryGetValue("class", out var classAttr))
        {
            Attributes.Add("class", new HtmlAttribute("class", className));
        }
        else
        {
            if (classAttr.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).All(x => x != className))
            {
                classAttr.Value += $" {classAttr}";
            }
        }
        return this;
    }

    public HtmlElement AddAttributeIfNotEmpty(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return this;
        }
        return AddAttribute(name, value);
    }

    public HtmlElement SetAttribute(string name, string value = null)
    {
        Attributes[name] = new HtmlAttribute(name, value);
        return this;
    }

    public HtmlAttribute GetAttribute(string name)
    {
        return Attributes.TryGetValue(name, out var attribute) ? attribute : null;
    }

    public bool HasAttribute(string name, string value = null)
    {
        return Attributes.TryGetValue(name, out var attribute) && (value == null || attribute.Value == value);
    }

    public HtmlElement WithText(string text)
    {
        Text = text;
        return this;
    }


    public HtmlElement AddAbove(IRazorFileNode node)
    {
        Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this), node);
        return this;
    }

    public HtmlElement AddAbove(params IRazorFileNode[] nodes)
    {
        foreach (var node in nodes)
        {
            AddAbove(node);
        }
        return this;
    }

    public HtmlElement AddBelow(IRazorFileNode node)
    {
        Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this) + 1, node);
        return this;
    }

    public HtmlElement AddBelow(params IRazorFileNode[] nodes)
    {
        foreach (var node in nodes.Reverse())
        {
            AddBelow(node);
        }
        return this;
    }

    public void Remove()
    {
        Parent.ChildNodes.Remove(this);
    }

    public override string GetText(string indentation)
    {
        var sb = new StringBuilder();
        var requiresEndTag = !string.IsNullOrWhiteSpace(Text) || ChildNodes.Any() || Name is "script";
        sb.Append($"{indentation}<{Name}{FormatAttributes(indentation)}{(!requiresEndTag ? " /" : "")}>{(requiresEndTag && (ChildNodes.Any() || Attributes.Count > 1) ? Environment.NewLine : "")}");

        foreach (var e in ChildNodes)
        {
            sb.Append(e.GetText($"{indentation}    "));
        }

        if (!string.IsNullOrWhiteSpace(Text))
        {
            if (ChildNodes.Any() || Attributes.Count > 1)
            {
                sb.AppendLine($"{indentation}    {Text}");
            }
            else
            {
                sb.Append(Text);
            }
        }

        if (requiresEndTag)
        {
            if (Attributes.Count > 1 || ChildNodes.Any())
            {
                sb.Append($"{(!ChildNodes.Any() && Attributes.Count <= 1 ? Environment.NewLine : "")}{indentation}</{Name}>");
            }
            else
            {
                sb.Append($"</{Name}>");
            }
        }

        sb.AppendLine();

        return sb.ToString();
    }

    private string FormatAttributes(string indentation)
    {
        var separateLines = Name is not "link" and not "meta";
        return string.Join(separateLines 
                ? $"{Environment.NewLine}{indentation}{new string(' ', Name.Length + 1)}" 
                : "", 
            Attributes.Values.Select(attribute => $" {attribute.Name}{(attribute.Value != null ? $"=\"{attribute.Value}\"" : "")}"));
    }

    public override string ToString()
    {
        return GetText("");
    }
}