using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

public abstract class DataFileWriter : IDataFileValueVisitor
{
    protected readonly StringBuilder StringBuilder = new();
    private readonly Stack<string> _indentationStack = new();
    private readonly string _tab;

    protected DataFileWriter(string tab, string commentSyntax)
    {
        _tab = tab;
        CommentSyntax = commentSyntax;
        UpdateComputedIndentations();
    }

    public void Visit(IDataFileValue value)
    {
        switch (value)
        {
            case IDataFileDictionaryValue castValue:
                Visit(castValue);
                break;
            case IDataFileArrayValue castValue:
                Visit(castValue);
                break;
            case IDataFileScalarValue castValue:
                Visit(castValue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), $"Unknown value type: {value.GetType()}");
        }
    }

    protected string CommentSyntax { get; }
    protected string PreviousIndentation { get; private set; }
    protected string Indentation { get; private set; }
    public abstract void Visit(IDataFileDictionaryValue dictionary);
    public abstract void Visit(IDataFileArrayValue array);
    public abstract void Visit(IDataFileScalarValue scalar);

    protected void PushIndentation(string content = null)
    {
        _indentationStack.Push(content ?? _tab);
        UpdateComputedIndentations();
    }

    protected void PopIndentation()
    {
        _indentationStack.Pop();
        UpdateComputedIndentations();
    }

    private void UpdateComputedIndentations()
    {
        if (_indentationStack.Count == 0)
        {
            PreviousIndentation = string.Empty;
            Indentation = string.Empty;
            return;
        }

        var reversed = _indentationStack.Reverse().ToArray();

        PreviousIndentation = string.Concat(reversed.Take(reversed.Length - 1));
        Indentation = string.Concat(reversed);
    }

    public override string ToString()
    {
        return StringBuilder.ToString();
    }
}