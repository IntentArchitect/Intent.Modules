using System;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpSwitchElement : CSharpStatement
{
    private readonly CSharpSwitchCodeBlock _codeBlock;

    public CSharpSwitchElement(string keyword, string value, Action<CSharpSwitchCodeBlock> codeBlock) : base(string.Empty)
    {
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.None;
        Text = $"{keyword}{(!string.IsNullOrWhiteSpace(value) ? " " + value : string.Empty)}:";
        if (codeBlock is null)
        {
            return;
        }
        _codeBlock = new CSharpSwitchCodeBlock();
        codeBlock(_codeBlock);
    }
    
    public override string GetText(string indentation)
    {
        var sb = new StringBuilder(128);
        if (Text.Length > 0)
        {
            sb.Append(base.GetText(indentation));
        }
        if (_codeBlock is not null)
        {
            sb.Append(_codeBlock.GetText(indentation));
        }
        return sb.ToString();
    }
}