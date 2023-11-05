using System;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFile : IFileBuilderBase<IDataFile>
{
    internal Func<DataFileWriter> WriterProvider { get; }
    IDataFileDictionaryValue RootDictionary { get; }
    IDataFile WithJsonWriter();
    IDataFile WithYamlWriter(bool alwaysQuoteStrings = false);
    IDataFile WithOclWriter();
    IDataFile WithRootDictionary(IDataFileBuilderTemplate template, Action<IDataFileDictionaryValue> configure);
    IDataFile WithWriter(Func<DataFileWriter> writerProvider, string fileExtension);
}