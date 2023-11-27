using System;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public class IndentedFile : FileBuilderBase<IIndentedFile>, IIndentedFile
{
    private IIndentedFileItems _items;

    public IndentedFile(
        string fileName,
        string extension = null,
        string relativeLocation = null,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always)
        : base(fileName, relativeLocation, extension, overwriteBehaviour)
    {
    }

    public IIndentedFileItems Items
    {
        get => _items;
        private set => _items = value ?? throw new InvalidOperationException($"{nameof(Items)} is not set, ensure the {nameof(WithItems)} has been called");
    }

    public string Indentation { get; set; } = "  ";

    public IIndentedFile WithItems(Action<IIndentedFileItems> configure)
    {
        Items = new IndentedFileItems();
        OnBuild(_ => configure(Items));
        return this;
    }

    public IIndentedFile WithIndentation(string indentation)
    {
        Indentation = indentation;
        return this;
    }

    public override string ToString()
    {
        return Items.ToString(Indentation);
    }
}