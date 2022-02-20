using System;
using Intent.Engine;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public interface ICSharpProject : IOutputTarget
    {
        string LanguageVersion { get; }
        bool NullableEnabled { get; }
        bool IsNetCore2App { get; }
        bool IsNetCore3App { get; }
        bool IsNet4App { get; }
        bool IsNet5App { get; }
        bool IsNet6App { get; }

        Version[] TargetDotNetFrameworks { get; }
        bool IsNullableAwareContext();
    }
}