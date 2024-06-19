namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpAttribute : ICSharpMetadataBase, IHasCSharpStatementsActual
{
    string Name { get; set; }
    ICSharpAttribute AddArgument(string name);
    ICSharpAttribute FindAndReplace(string find, string replaceWith);
}