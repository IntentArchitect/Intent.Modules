using System;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.AppStartup;

/// <summary>
/// Abstracts working with methods for <c>Program.cs</c> file as depending on whether or not the use
/// <see href="https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements">top-level statements</see>
/// option has been selected, the ultimate types used will be a <see cref="CSharpClassMethod"/> or <see cref="CSharpLocalMethod"/>.
/// </summary>
public interface IStartupMethod : IHasCSharpStatements
{
    IStartupMethod Static();
    IStartupMethod Async();
    IStartupMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null);
    IStartupMethod AddGenericParameter(string typeName);
    IStartupMethod AddGenericParameter(string typeName, out CSharpGenericParameter param);
    IStartupMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure);
}