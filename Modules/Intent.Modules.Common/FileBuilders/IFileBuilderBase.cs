using System.Collections.Generic;
using System;

namespace Intent.Modules.Common.FileBuilders;

public interface IFileBuilderBase
{
    protected internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationDelegates();
    protected internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationAfterDelegates();
    protected internal void MarkCompleteBuildAsDone();
    protected internal void MarkAfterBuildAsDone();
}