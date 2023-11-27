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

        Version[] TargetDotNetFrameworks { get; }
        bool IsNullableAwareContext();
    }
}