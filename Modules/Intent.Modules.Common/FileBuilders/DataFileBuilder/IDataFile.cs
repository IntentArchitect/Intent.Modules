using System;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFile : IFileBuilderBase<IDataFile>
{
    internal Func<DataFileWriter> WriterProvider { get; }
    IDataFileObjectValue RootObject { get; }
    IDataFile WithJsonWriter();
    IDataFile WithYamlWriter(bool alwaysQuoteStrings = false);
    IDataFile WithOclWriter();
    IDataFile WithRootObject(IDataFileBuilderTemplate template, Action<IDataFileObjectValue> configure);
    IDataFile WithWriter(Func<DataFileWriter> writerProvider, string fileExtension);
}