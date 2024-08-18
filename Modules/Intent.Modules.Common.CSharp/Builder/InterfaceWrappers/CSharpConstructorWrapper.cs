using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpConstructorWrapper(CSharpConstructor wrapped) :
    CSharpMemberWrapper<CSharpConstructor, ICSharpConstructor>(wrapped), ICSharpConstructor
{
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);

    ICSharpClass ICSharpConstructor.Class => wrapped.Class;

    string ICSharpConstructor.AccessModifier => wrapped.AccessModifier;

    ICSharpConstructorCall ICSharpConstructor.ConstructorCall => wrapped.ConstructorCall;

    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;

    string IHasCSharpName.Name => wrapped.Name;

    bool ICSharpConstructor.IsPrimaryConstructor => wrapped.IsPrimaryConstructor;

    ICSharpConstructor ICSharpConstructor.AddParameter<TModel>(string type, TModel model, Action<ICSharpConstructorParameter> configure) => wrapped.AddParameter(type, model, configure);

    ICSharpConstructor ICSharpConstructor.AddParameter<TModel>(TModel model, Action<ICSharpConstructorParameter> configure) => wrapped.AddParameter(model, configure);

    ICSharpConstructor ICSharpConstructor.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpConstructorParameter> configure) => wrapped.AddParameters(models, configure);

    ICSharpConstructor ICSharpConstructor.AddParameter(string type, string name, Action<ICSharpConstructorParameter> configure) => wrapped.AddParameter(type, name, configure);

    ICSharpConstructor ICSharpConstructor.InsertParameter(int index, string type, string name, Action<ICSharpConstructorParameter> configure) => wrapped.InsertParameter(index, type, name, configure);

    ICSharpConstructor ICSharpConstructor.AddStatement(ICSharpStatement statement, Action<ICSharpStatement> configure) => wrapped.AddStatement((CSharpStatement)statement, configure);

    ICSharpConstructor ICSharpConstructor.InsertStatement(int index, string statement, Action<ICSharpStatement> configure) => wrapped.InsertStatement(index, statement, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements.Cast<CSharpStatement>(), configure);

    ICSharpConstructor ICSharpConstructor.Protected() => wrapped.Protected();

    ICSharpConstructor ICSharpConstructor.Private() => wrapped.Private();

    ICSharpConstructor ICSharpConstructor.Static() => wrapped.Static();

    ICSharpConstructor ICSharpConstructor.CallsBase(Action<ICSharpConstructorCall> configure) => wrapped.CallsBase(configure);

    ICSharpConstructor ICSharpConstructor.CallsThis(Action<ICSharpConstructorCall> configure) => wrapped.CallsThis(configure);
}