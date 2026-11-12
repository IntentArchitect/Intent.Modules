using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftReporting.Api
{
    /// <summary>What a finding compares.</summary>
    public enum DriftCategory
    {
        /// <summary>The model and the code disagree.</summary>
        ModelCode,

        /// <summary>Code breaks a rule about other code, e.g. a Layer importing one it may not depend on.</summary>
        CodeCode,

        /// <summary>The code alone has a problem, e.g. a file no convention covers, or a check's own configuration.</summary>
        Code
    }

    public enum DriftSeverity
    {
        /// <summary>Fails the run.</summary>
        Error,

        /// <summary>Is reported, but never fails the run.</summary>
        Warning
    }

    /// <summary>One row of the drift report.</summary>
    public sealed class DriftFinding
    {
        /// <param name="category">What the finding compares.</param>
        /// <param name="severity">Whether it fails the run.</param>
        /// <param name="message">What is wrong, as plain text: a backtick span renders as inline code.</param>
        /// <param name="subject">What the finding is about.</param>
        /// <param name="against">What the subject was compared with, if anything.</param>
        public DriftFinding(DriftCategory category, DriftSeverity severity, string message, DriftAnchor subject, params DriftAnchor[] against)
        {
            Category = category;
            Severity = severity;
            Message = message ?? string.Empty;
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Against = (against ?? Array.Empty<DriftAnchor>()).Where(x => x != null).ToList();
        }

        public DriftCategory Category { get; }
        public DriftSeverity Severity { get; }
        public string Message { get; }
        public DriftAnchor Subject { get; }
        public IReadOnlyList<DriftAnchor> Against { get; }

        /// <summary>An optional stable id for the kind of finding, e.g. <c>arch.layer-dependency</c>.</summary>
        public string Rule { get; set; }
    }

    public enum DriftAnchorKind
    {
        Element,
        File,
        Text
    }

    /// <summary>A model element, a file, or anything else a finding is about or was compared with.</summary>
    public sealed class DriftAnchor
    {
        private DriftAnchor(DriftAnchorKind kind, IElement element, string path, int? line, string text)
        {
            Kind = kind;
            ModelElement = element;
            FilePath = path;
            Line = line;
            Value = text;
        }

        /// <summary>A model element: the report links to it, and an Error's card is on it.</summary>
        public static DriftAnchor Element(IElement element) =>
            new(DriftAnchorKind.Element, element ?? throw new ArgumentNullException(nameof(element)), null, null, null);

        /// <summary>A file, and optionally a line in it.</summary>
        /// <param name="path">Absolute, or relative to the application's output root.</param>
        /// <param name="line">1-based; <see langword="null"/> or 0 for the file as a whole.</param>
        public static DriftAnchor File(string path, int? line = null) =>
            new(DriftAnchorKind.File, null, path ?? throw new ArgumentNullException(nameof(path)), line is > 0 ? line : null, null);

        /// <summary>Anything else, as plain text: a folder, a rule, a missing file. A backtick span renders as inline code.</summary>
        public static DriftAnchor Text(string text) => new(DriftAnchorKind.Text, null, null, null, text ?? string.Empty);

        public DriftAnchorKind Kind { get; }

        /// <summary>For <see cref="DriftAnchorKind.Element"/>.</summary>
        public IElement ModelElement { get; }

        /// <summary>For <see cref="DriftAnchorKind.File"/>.</summary>
        public string FilePath { get; }

        /// <summary>For <see cref="DriftAnchorKind.File"/>, when it names a line.</summary>
        public int? Line { get; }

        /// <summary>For <see cref="DriftAnchorKind.Text"/>.</summary>
        public string Value { get; }
    }
}
