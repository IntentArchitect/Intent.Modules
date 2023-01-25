using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaClassMethod: JavaMember<JavaClassMethod>, IHasJavaStatements
{
    public IList<JavaStatement> Statements { get; } = new List<JavaStatement>();
    protected string AsyncMode { get; private set; } = string.Empty;
    protected string AccessModifier { get; private set; } = "public ";
    protected string OverrideModifier { get; private set; } = string.Empty;
    public string ReturnType { get; }
    public string Name { get; }
    public List<JavaParameter> Parameters { get; } = new();
    // public IList<JavaGenericParameter> GenericParameters { get; } = new List<JavaGenericParameter>();
    // public IList<JavaGenericTypeConstraint> GenericTypeConstraints { get; } = new List<JavaGenericTypeConstraint>();
    
    public JavaClassMethod(string returnType, string name)
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
        BeforeSeparator = JavaCodeSeparatorType.EmptyLines;
        AfterSeparator = JavaCodeSeparatorType.EmptyLines;
    }

    public JavaClassMethod AddParameter(string type, string name, Action<JavaParameter> configure = null)
    {
        var param = new JavaParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }
    
    // public JavaClassMethod AddGenericParameter(string typeName)
    // {
    //     var param = new JavaGenericParameter(typeName);
    //     GenericParameters.Add(param);
    //     return this;
    // }
    //
    // public JavaClassMethod AddGenericParameter(string typeName, out JavaGenericParameter param)
    // {
    //     param = new JavaGenericParameter(typeName);
    //     GenericParameters.Add(param);
    //     return this;
    // }
    //
    // public JavaClassMethod AddGenericTypeConstraint(string genericParameterName, Action<JavaGenericTypeConstraint> configure)
    // {
    //     var param = new JavaGenericTypeConstraint(genericParameterName);
    //     configure(param);
    //     GenericTypeConstraints.Add(param);
    //     return this;
    // }

    public JavaClassMethod AddStatement(string statement, Action<JavaStatement> configure = null)
    {
        return AddStatement(new JavaStatement(statement), configure);
    }

    public JavaClassMethod AddStatement(JavaStatement statement, Action<JavaStatement> configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public JavaClassMethod InsertStatement(int index, JavaStatement statement, Action<JavaStatement> configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public JavaClassMethod InsertStatements(int index, IReadOnlyCollection<JavaStatement> statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public JavaClassMethod AddStatements(string statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public JavaClassMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new JavaStatement(x)), configure);
    }

    public JavaClassMethod AddStatements(IEnumerable<JavaStatement> statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
            statement.Parent = this;
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public JavaClassMethod FindAndReplaceStatement(Func<JavaStatement, bool> matchFunc, JavaStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public JavaClassMethod Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public JavaClassMethod Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public JavaClassMethod WithoutAccessModifier()
    {
        AccessModifier = "";
        return this;
    }

    public JavaClassMethod Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public JavaClassMethod New()
    {
        OverrideModifier = "new ";
        return this;
    }

    public JavaClassMethod Virtual()
    {
        OverrideModifier = "virtual ";
        return this;
    }

    public JavaClassMethod Abstract()
    {
        OverrideModifier = "abstract ";
        return this;
    }

    public JavaClassMethod Static()
    {
        OverrideModifier = "static ";
        return this;
    }

    public JavaClassMethod Async()
    {
        AsyncMode = "async ";
        return this;
    }

    public void RemoveStatement(JavaStatement statement)
    {
        Statements.Remove(statement);
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAnnotations(indentation)}{indentation}{AccessModifier}{OverrideModifier}{AsyncMode}{ReturnType} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))})
{indentation}{{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }
    
//     private string GetGenericTypeConstraints(string indentation)
//     {
//         if (!GenericTypeConstraints.Any())
//         {
//             return string.Empty;
//         }
//
//         string newLine = $@"
// {indentation}    ";
//         return newLine + string.Join(newLine, GenericTypeConstraints);
//     }
//     
//     private string GetGenericParameters()
//     {
//         if (!GenericParameters.Any())
//         {
//             return string.Empty;
//         }
//
//         return $"<{string.Join(", ", GenericParameters)}>";
//     }
}