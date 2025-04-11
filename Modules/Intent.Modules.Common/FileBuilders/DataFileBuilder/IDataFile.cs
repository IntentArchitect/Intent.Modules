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

    /// <summary>
    /// Use one of the following values:
    /// <list type="bullet">
    /// <item><see cref="CustomMetadataValues.MergeModeFully"/></item>
    /// <item><see cref="CustomMetadataValues.MergeModeIgnore"/></item>
    /// <item><see cref="CustomMetadataValues.MergeModeMerge"/></item>
    /// </list>
    /// </summary>
    IDataFile WithDefaultMergeMode(string mode);
}