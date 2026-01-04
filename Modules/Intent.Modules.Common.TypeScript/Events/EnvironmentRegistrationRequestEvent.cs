using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Events;

/// <summary>
/// The type of the environment configuration being registered.
/// </summary>
public enum EnvironmentTypeKind
{
    /// <summary>
    /// string | number | boolean
    /// </summary>
    Simple,
    /// <summary>
    /// A complex type which will be output as a TypeScript interface.
    /// </summary>
    Interface,
    /// <summary>
    /// An alias for an existing type or a complex type expression. e.g. type FeatureFlags = Record&lt;string,boolean&gt;
    /// </summary>
    TypeAlias
}

/// <summary>
/// Represents a field/property on an environment configuration type.
/// </summary>
public sealed class EnvironmentFieldDescriptor
{
    /// The TS property name: "baseUrl", "timeoutMs", etc.
    public required string Name { get; init; }

    /// TS type: "string", "number", "boolean", "HttpConfig", "Record&lt;string,boolean&gt;", etc.
    public required string Type { get; init; }

    /// Whether the property should be optional in TS: "prop?: Type"
    public bool IsOptional { get; init; } = false;

    /// Optional default value for this field, expressed as a TypeScript literal or
    /// expression, e.g. "'https://localhost:5001/'", "5000", "true".
    public string? DefaultValue { get; init; }
}

/// <summary>
/// The actual event to be published to register an environment configuration type.
/// </summary>
public sealed class EnvironmentRegistrationRequestEvent
{
    /// <summary>
    ///  The optional name of the field to add to AppEnvironment
    /// </summary>
    public string? EnvironmentName { get; init; }

    /// Name of the TypeScript type to generate, e.g. "CustomerServiceConfig".
    /// For Simple kinds you may omit this and just use SimpleType.
    public string? TypeName { get; set; }

    /// <summary>
    /// The kind of TypeScript construct to generate.
    /// </summary>
    public EnvironmentTypeKind Kind { get; init; } = EnvironmentTypeKind.Interface;

    /// <summary>
    /// For Simple and TypeAlias kinds, the TypeScript type expression to use,
    /// e.g. "string", "number", "boolean", "Record&lt;string, boolean&gt;".
    /// For Interface kinds this may be null.
    /// </summary>
    public string? SimpleType { get; init; }

    /// <summary>
    /// Fields for an Interface kind. Ignored for Simple and TypeAlias kinds.
    /// </summary>
    public IList<EnvironmentFieldDescriptor>? Fields { get; init; }

    /// Base interfaces this type should extend.
    /// For Kind == Interface, generates: interface X extends A, B { ... }
    /// Ignored for Simple and TypeAlias.
    public IList<string>? Extends { get; init; }

    /// <summary>
    /// Optional default value to emit for this environment path in environment.ts,
    /// expressed as a TypeScript expression.
    /// Examples:
    ///   "{ baseUrl: 'https://localhost:5001/', timeoutMs: 5000 }"
    ///   "'dev-local'"
    ///   "true"
    ///
    /// If null, the generator can derive an object literal from field-level
    /// DefaultValue values, or leave the property uninitialized.
    /// </summary>
    public string? DefaultValue { get; init; }

    /// <summary>
    /// Comments to be added to the generated type.
    /// </summary>
    public string? Comment { get; init; }

}



