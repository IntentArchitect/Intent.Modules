namespace Humanizer
{
    /// <summary>
    /// Can transform a string
    /// </summary>
    internal interface IStringTransformer
    {
        /// <summary>
        /// Transform the input
        /// </summary>
        /// <param name="input">String to be transformed</param>
        /// <returns></returns>
        string Transform(string input);
    }
}