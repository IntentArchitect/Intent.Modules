using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpConstructor : ICSharpMember<ICSharpConstructor>, IHasCSharpStatementsActual, IHasICSharpParameters, ICSharpReferenceable {
    ICSharpClass Class { get; }
    string AccessModifier { get; }
    ICSharpConstructorCall ConstructorCall { get; }
    bool IsPrimaryConstructor { get; }
    ICSharpConstructor AddParameter<TModel>(string type, TModel model, Action<ICSharpConstructorParameter> configure = null) where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpConstructor AddParameter<TModel>(TModel model, Action<ICSharpConstructorParameter> configure = null) where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpConstructor AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpConstructorParameter> configure = null) where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpConstructor AddParameter(string type, string name, Action<ICSharpConstructorParameter> configure = null);
    ICSharpConstructor InsertParameter(int index, string type, string name, Action<ICSharpConstructorParameter> configure = null);
    ICSharpConstructor AddStatement(ICSharpStatement statement, Action<ICSharpStatement> configure = null);
    ICSharpConstructor InsertStatement(int index, string statement, Action<ICSharpStatement> configure = null);
    ICSharpConstructor AddStatements(string statements, Action<IEnumerable<ICSharpStatement>> configure = null);
    ICSharpConstructor AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>> configure = null);
    ICSharpConstructor AddStatements(IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure = null);
    ICSharpConstructor Protected();
    ICSharpConstructor Private();
    ICSharpConstructor Internal();
    ICSharpConstructor Static();
    ICSharpConstructor CallsBase(Action<ICSharpConstructorCall> configure = null);
    ICSharpConstructor CallsThis(Action<ICSharpConstructorCall> configure = null);
}