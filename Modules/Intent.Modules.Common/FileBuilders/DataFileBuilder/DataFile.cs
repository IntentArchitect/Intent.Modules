using System;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public class DataFile : FileBuilderBase<IDataFile>, IDataFile
{
    private IDataFileDictionaryValue _rootElement;

    public DataFile(
        string fileName,
        string relativeLocation = null,
        string extension = null,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always)
        : base(fileName, relativeLocation, extension, overwriteBehaviour)
    {
    }

    public Func<DataFileWriter> WriterProvider { get; private set; } = () => new DataFileJsonWriter();

    public IDataFileDictionaryValue RootDictionary
    {
        get => _rootElement;
        private set => _rootElement = value ?? throw new InvalidOperationException($"{nameof(RootDictionary)} is not set, ensure the {nameof(WithRootDictionary)} has been called");
    }

    public IDataFile WithJsonWriter() => WithWriter(() => new DataFileJsonWriter(), "json");

    public IDataFile WithYamlWriter(bool alwaysQuoteStrings = false) => WithWriter(() => new DataFileYamlWriter(alwaysQuoteStrings), "yaml");

    public IDataFile WithOclWriter() => WithWriter(() => new DataFileOclWriter(), "ocl");

    public IDataFile WithWriter(Func<DataFileWriter> writerProvider, string fileExtension)
    {
        WriterProvider = writerProvider;
        return WithFileExtension(fileExtension);
    }

    public IDataFile WithRootDictionary(IDataFileBuilderTemplate template, Action<IDataFileDictionaryValue> configure)
    {
        var dataFileDictionaryValue = new DataFileDictionaryValue();
        dataFileDictionaryValue.AttachToParent(template: template, parent: null);
        RootDictionary = dataFileDictionaryValue;
        OnBuild(_ => configure(dataFileDictionaryValue));
        return this;
    }

    public override string ToString()
    {
        return RootDictionary.ToString();
    }
}