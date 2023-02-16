using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Java.Templates;

namespace Intent.Modules.Common.Java.Builder;

public class JavaInterface : JavaDeclaration<JavaInterface>
{
    private JavaCodeSeparatorType _fieldsSeparator = JavaCodeSeparatorType.NewLine;
    private JavaCodeSeparatorType _propertiesSeparator = JavaCodeSeparatorType.NewLine;
    private JavaCodeSeparatorType _methodsSeparator = JavaCodeSeparatorType.NewLine;

    public JavaInterface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.ToJavaIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper);
    }

    public string Name { get; }
    protected string AccessModifier { get; private set; } = "public ";
    public IList<string> Interfaces { get; set; } = new List<string>();
    public IList<JavaInterfaceField> Fields { get; } = new List<JavaInterfaceField>();
    public IList<JavaInterfaceMethod> Methods { get; } = new List<JavaInterfaceMethod>();

    public IList<JavaCodeBlock> CodeBlocks { get; } = new List<JavaCodeBlock>();
    // public IList<JavaInterfaceGenericParameter> GenericParameters { get; } = new List<JavaInterfaceGenericParameter>();
    // public IList<JavaGenericTypeConstraint> GenericTypeConstraints { get; } = new List<JavaGenericTypeConstraint>();

    public JavaInterface ExtendsInterface(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        Interfaces.Add(type);
        return this;
    }

    public JavaInterface ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public JavaInterface AddField(string type, string name, Action<JavaInterfaceField> configure = null)
    {
        var field = new JavaInterfaceField(type, name)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public JavaInterface AddMethod(string returnType, string name, Action<JavaInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public JavaInterface AddCodeBlock(string codeLine)
    {
        CodeBlocks.Add(new JavaCodeBlock(codeLine));
        return this;
    }

    // public JavaInterface AddGenericParameter(string typeName, Action<JavaInterfaceGenericParameter> configure = null)
    // {
    //     var param = new JavaInterfaceGenericParameter(typeName);
    //     configure?.Invoke(param);
    //     GenericParameters.Add(param);
    //     return this;
    // }
    //
    // public JavaInterface AddGenericParameter(string typeName, out JavaInterfaceGenericParameter param, Action<JavaInterfaceGenericParameter> configure = null)
    // {
    //     param = new JavaInterfaceGenericParameter(typeName);
    //     configure?.Invoke(param);
    //     GenericParameters.Add(param);
    //     return this;
    // }
    
    // public JavaInterface AddGenericTypeConstraint(string genericParameterName, Action<JavaGenericTypeConstraint> configure)
    // {
    //     var param = new JavaGenericTypeConstraint(genericParameterName);
    //     configure(param);
    //     GenericTypeConstraints.Add(param);
    //     return this;
    // }

    public JavaInterface InsertMethod(int index, string returnType, string name, Action<JavaInterfaceMethod> configure = null)
    {
        var method = new JavaInterfaceMethod(returnType, name)
        {
            BeforeSeparator = _methodsSeparator,
            AfterSeparator = _methodsSeparator
        };
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public JavaInterface WithFieldsSeparated(JavaCodeSeparatorType separator = JavaCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public JavaInterface WithPropertiesSeparated(JavaCodeSeparatorType separator = JavaCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        return this;
    }

    public JavaInterface WithMethodsSeparated(JavaCodeSeparatorType separator = JavaCodeSeparatorType.EmptyLines)
    {
        _methodsSeparator = separator;
        return this;
    }

    public JavaInterface WithMembersSeparated(JavaCodeSeparatorType separator = JavaCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        _methodsSeparator = separator;
        return this;
    }

    public JavaInterface Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public JavaInterface Private()
    {
        AccessModifier = "private ";
        return this;
    }


    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        return $@"{GetAnnotations(indentation)}{indentation}{AccessModifier}interface {Name}{GetBaseTypes()} {{{GetMembers($"{indentation}    ")}
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
//         return $"<{string.Join(", ", GenericParameters.Select(s => s.GetText()))}>";
//     }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        foreach (var @interface in Interfaces)
        {
            types.Add(@interface);
        }

        return types.Any() ? $" extends {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);
        codeBlocks.AddRange(Methods);
        codeBlocks.AddRange(CodeBlocks);

        return !codeBlocks.Any() ? string.Empty : $@"
{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }
}