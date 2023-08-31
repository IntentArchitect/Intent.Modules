using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaInterfaceMethod : JavaMember<JavaInterfaceMethod>, IHasJavaStatements
{
    public IList<JavaStatement> Statements { get; } = new List<JavaStatement>();
    public string ReturnType { get; }
    public string Name { get; }
    public IList<JavaParameter> Parameters { get; } = new List<JavaParameter>();
    public IList<JavaGenericParameter> GenericParameters { get; } = new List<JavaGenericParameter>();
    public IList<string> CheckedExceptions { get; } = new List<string>();

    public JavaInterfaceMethod(string returnType, string name)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        ReturnType = returnType;
        Name = name;
        BeforeSeparator = JavaCodeSeparatorType.NewLine;
        AfterSeparator = JavaCodeSeparatorType.NewLine;
    }

    public JavaInterfaceMethod AddParameter(string type, string name, Action<JavaParameter> configure = null)
    {
        var param = new JavaParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public JavaInterfaceMethod AddGenericParameter(string typeName)
    {
        var param = new JavaGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public JavaInterfaceMethod AddGenericParameter(string typeName, out JavaGenericParameter param)
    {
        param = new JavaGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }
    
    public JavaInterfaceMethod Throws(string exceptionType)
    {
        CheckedExceptions.Add(exceptionType);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAnnotations(indentation)}{indentation}{GetDefaultKeyword()}{GetGenericParameters()}{ReturnType} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))}){GetCheckedExceptions()}{GetMethodBody(indentation)};";
    }

    private string GetDefaultKeyword()
    {
        return Statements.Any() ? "default " : string.Empty;
    }
    
    private string GetCheckedExceptions()
    {
        return !CheckedExceptions.Any() 
            ? string.Empty 
            : $" throws {string.Join(", ", CheckedExceptions)} ";
    }
    
    private string GetMethodBody(string indentation)
    {
        return !Statements.Any() 
            ? string.Empty 
            : $@" {{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private string GetGenericParameters()
    {
        return !GenericParameters.Any() 
            ? string.Empty 
            : $"<{string.Join(", ", GenericParameters)}>";
    }
}