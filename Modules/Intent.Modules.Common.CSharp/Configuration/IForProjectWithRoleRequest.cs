namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// Used for requests which should only apply to template instances under a project with a
    /// particular output target 'Role' present under it within Intent Architect Visual Studio
    /// designer.
    /// </summary>
    /// <remarks>
    /// Used for disambiguating which file to apply to when a solution has multiple
    /// projects each with their own file.
    /// </remarks>
    public interface IForProjectWithRoleRequest
    {
        /// <summary>
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// </summary>
        /// <remarks>
        /// Used for disambiguating which file to apply to when a solution has multiple
        /// projects each with their own file.
        /// </remarks>
        string ForProjectWithRole { get; }

        /// <summary>
        /// Whether or not <see cref="MarkHandled"/> has been called.
        /// </summary>
        bool WasHandled { get; }

        /// <summary>
        /// Sets <see cref="WasHandled"/> to <see langword="true"/> if it wasn't already.
        /// </summary>
        /// <remarks>
        /// Should be called by handlers so that publishers can query <see cref="WasHandled"/>
        /// and potentially show warnings when its value is <see langword="false"/>.
        /// </remarks>
        void MarkHandled();
    }
}
