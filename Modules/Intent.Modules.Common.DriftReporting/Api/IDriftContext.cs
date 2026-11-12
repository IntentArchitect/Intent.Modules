using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modules.Common.DriftReporting.Api
{
    /// <summary>What a reporter gets to report with: the application, and the report's Areas.</summary>
    public interface IDriftContext
    {
        /// <summary>The application: its metadata, output root, template instances and configuration.</summary>
        IApplication Application { get; }

        /// <summary>
        /// The Area with the given name, created the first time it is asked for. Areas appear in the report in that order,
        /// and reporters that name the same Area share it.
        /// </summary>
        /// <param name="name">The Area's name, as plain text, e.g. <c>Architecture</c>.</param>
        /// <param name="checks">
        /// The categories this reporter checks in the Area. A declared category with no findings shows as clean; one that
        /// isn't declared and has no findings is left out. Reporters that share an Area add their declarations together.
        /// </param>
        IDriftArea Area(string name, params DriftCategory[] checks);
    }

    /// <summary>One section of the drift report, e.g. Architecture or Domain.</summary>
    public interface IDriftArea
    {
        /// <summary>Adds a finding. An Error fails the run; a Warning never does.</summary>
        void Log(DriftFinding finding);
    }
}
