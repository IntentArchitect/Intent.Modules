namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// Controls casing behaviour of <see cref="TemplateExtensions.ToCSharpIdentifier(string,CapitalizationBehaviour)"/>
    /// </summary>
    public enum CapitalizationBehaviour
    {
        /// <summary>
        /// Do not change casing.
        /// </summary>
        AsIs = 1,

        /// <summary>
        /// Ensures that first letter is lowercase, i.e., the result will be
        /// <see href="https://en.wikipedia.org/wiki/Camel_case">camelCased</see>.
        /// </summary>
        MakeFirstLetterLower = 2,

        /// <summary>
        /// Ensures that first letter is uppercase, i.e., the result will be
        /// <see href="https://en.wikipedia.org/wiki/Pascal_case">PascalCased</see>.
        /// </summary>
        MakeFirstLetterUpper = 3
    }
}