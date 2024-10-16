using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public class IndentedFileItems : IndentedFileItem<IIndentedFileItems>, IIndentedFileItems
{
    public List<IIndentedFileItem> Items { get; } = new();

    public IIndentedFileItems WithItems(Action<IIndentedFileItems> configure)
    {
        var items = new IndentedFileItems();
        Items.Add(items);
        configure(items);
        return this;
    }

    public IIndentedFileItems WithContent(string content, Action<IIndentedFileContent> configure = null)
    {
        var contentItem = new IndentedFileContent(content);
        Items.Add(contentItem);
        configure?.Invoke(contentItem);
        return this;
    }

    public string ToString(string indentation)
    {
        var sb = new StringBuilder();
        var currentIndentation = string.Empty;
        var stack = new Stack<string>();

        Process(this);

        void Process(IIndentedFileItems items)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case IIndentedFileItems childGroup:
                        stack.Push(indentation);
                        currentIndentation = string.Concat(stack);

                        Process(childGroup);

                        stack.Pop();
                        currentIndentation = string.Concat(stack);

                        break;
                    case IIndentedFileContent content:
                        var lines = content.Content
                            .ReplaceLineEndings("\n")
                            .Split('\n');

                        foreach (var line in lines)
                        {
                            sb.Append(currentIndentation);
                            sb.AppendLine(line);
                        }

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        return sb.ToString();
    }

    public override string ToString() => ToString("  ");

    #region IList<IIndentedFileItem> implementation

    IEnumerator<IIndentedFileItem> IEnumerable<IIndentedFileItem>.GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    void ICollection<IIndentedFileItem>.Add(IIndentedFileItem item)
    {
        Items.Add(item);
    }

    void ICollection<IIndentedFileItem>.Clear()
    {
        Items.Clear();
    }

    bool ICollection<IIndentedFileItem>.Contains(IIndentedFileItem item)
    {
        return Items.Contains(item);
    }

    void ICollection<IIndentedFileItem>.CopyTo(IIndentedFileItem[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    bool ICollection<IIndentedFileItem>.Remove(IIndentedFileItem item)
    {
        return Items.Remove(item);
    }

    int ICollection<IIndentedFileItem>.Count => Items.Count;

    bool ICollection<IIndentedFileItem>.IsReadOnly => false;

    int IList<IIndentedFileItem>.IndexOf(IIndentedFileItem item)
    {
        return Items.IndexOf(item);
    }

    void IList<IIndentedFileItem>.Insert(int index, IIndentedFileItem item)
    {
        Items.Insert(index, item);
    }

    void IList<IIndentedFileItem>.RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }

    IIndentedFileItem IList<IIndentedFileItem>.this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    #endregion
}