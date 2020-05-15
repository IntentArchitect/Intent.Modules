using System;

namespace Intent.Modules.Common.VisualStudio
{
    public interface IAssemblyReference : IEquatable<IAssemblyReference>
    {
        string Library { get; }
        string HintPath { get; }
        bool HasHintPath();
    }
}
