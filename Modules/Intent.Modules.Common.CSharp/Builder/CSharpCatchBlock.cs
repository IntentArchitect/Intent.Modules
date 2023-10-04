using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpCatchBlock : CSharpStatementBlock
{
    public CSharpCatchBlock() : this(null, null)
    {
    }

    public CSharpCatchBlock(string exceptionType, string parameterName)
    {
        ExceptionType = exceptionType;
        ParameterName = parameterName;
        Apply();
    }

    public string ExceptionType { get; private set; }
    public string ParameterName { get; private set; }
    public string WhenExpression { get; private set; }

    public CSharpCatchBlock WithExceptionType(string exceptionType)
    {
        ExceptionType = exceptionType;
        return Apply();
    }

    public CSharpCatchBlock WithParameterName(string parameterName)
    {
        ParameterName = parameterName;
        return Apply();
    }

    public CSharpCatchBlock WithWhenExpression(string expression)
    {
        WhenExpression = expression;
        return Apply();
    }

    private CSharpCatchBlock Apply()
    {
        var sb = new StringBuilder("catch");

        if (ExceptionType != null)
        {
            sb.Append(" (");
            sb.Append(ExceptionType);
            if (ParameterName != null)
            {
                sb.Append(" ");
                sb.Append(ParameterName);
            }
            sb.Append(")");
        }

        if (WhenExpression != null)
        {
            sb.Append(" when (");
            sb.Append(WhenExpression);
            sb.Append(")");
        }

        Text = sb.ToString();

        return this;
    }
}