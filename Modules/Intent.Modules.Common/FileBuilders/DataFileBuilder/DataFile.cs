using System;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

/// <inheritdoc cref="IDataFile" />
public class DataFile : FileBuilderBase<IDataFile>, IDataFile
{
    private IDataFileObjectValue _rootElement;

    /// <inheritdoc />
    public DataFile(
        string fileName,
        string relativeLocation = null,
        string extension = null,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always)
        : base(fileName, relativeLocation, extension, overwriteBehaviour)
    {
    }

    /// <inheritdoc />
    public Func<DataFileWriter> WriterProvider { get; private set; } = () => new DataFileJsonWriter();

    /// <inheritdoc />
    public IDataFileObjectValue RootObject
    {
        get => _rootElement;
        private set => _rootElement = value ?? throw new InvalidOperationException($"{nameof(RootObject)} is not set, ensure the {nameof(WithRootObject)} has been called");
    }

    /// <inheritdoc />
    public IDataFile WithJsonWriter()
    {
        CodeGenType = Common.CodeGenType.JsonMerger;
        return WithWriter(() => new DataFileJsonWriter(), "json");
    }

    /// <inheritdoc />
    public IDataFile WithYamlWriter(bool alwaysQuoteStrings = false) => WithWriter(() => new DataFileYamlWriter(alwaysQuoteStrings), "yaml");

    /// <inheritdoc />
    public IDataFile WithOclWriter() => WithWriter(() => new DataFileOclWriter(), "ocl");

    /// <inheritdoc />
    public IDataFile WithWriter(Func<DataFileWriter> writerProvider, string fileExtension)
    {
        WriterProvider = writerProvider;
        return WithFileExtension(fileExtension);
    }

    /// <inheritdoc />
    public IDataFile WithRootObject(IDataFileBuilderTemplate template, Action<IDataFileObjectValue> configure)
    {
        var dataFileObjectValue = new DataFileObjectValue();
        dataFileObjectValue.AttachToParent(template: template, parent: null);
        RootObject = dataFileObjectValue;
        OnBuild(_ => configure(dataFileObjectValue));
        return this;
    }

    /// <inheritdoc />
    public IDataFile WithDefaultMergeMode(string mode)
    {
        CustomMetadata.Add(CustomMetadataKeys.DefaultMergeMode, mode);
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return RootObject.ToString();
    }
}