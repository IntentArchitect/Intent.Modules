using System;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public interface ICSharpProject : IOutputTarget
    {
        string LanguageVersion { get; }
        bool NullableEnabled { get; }
        IElement InternalElement { get; }
        bool IsNetCore2App { get; }
        bool IsNetCore3App { get; }
        /// <summary>
        /// Obsolete. Use <see cref="IsNetApp"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        bool IsNet4App { get; }
        /// <summary>
        /// Obsolete. Use <see cref="IsNetApp"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        bool IsNet5App { get; }
        /// <summary>
        /// Obsolete. Use <see cref="IsNetApp"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        bool IsNet6App { get; }
        bool IsNetApp(byte version);

        /// <summary>
        /// Determines if the project targets at least one .NET 5+ framework and provides the highest
        /// version thereof.
        /// </summary>
        bool TryGetMaxNetAppVersion(out MajorMinorVersion majorMinorVersion);

        /// <summary>
        /// Returns the highest targeted .NET 5+ version of the project. If the project does not have
        /// a .NET5+ target then an exception is thrown.
        /// </summary>
        MajorMinorVersion GetMaxNetAppVersion();

        Version[] TargetDotNetFrameworks { get; }
        bool IsNullableAwareContext();

        MajorMinorVersion GetLanguageVersion();
    }
}