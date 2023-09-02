namespace Intent.Modules.Common.Java.Templates;

/// <summary>
/// Options for <see cref="JavaDependency.Scope"/>. See <see href="https://maven.apache.org/pom.html#dependencies"/> for more details on the scope value.
/// </summary>
public enum JavaDependencyScope
{
    /// <summary>
    /// The default scope, used if none is specified. Compile dependencies are available in all classpaths. Furthermore, those dependencies are propagated to dependent projects.
    /// </summary>
    Compile = 0,

    /// <summary>
    /// Much like <see cref="Compile"/>, but indicates you expect the JDK or a container to provide it at runtime. It is only available on the compilation and test classpath, and is not transitive.
    /// </summary>
    Provided,

    /// <summary>
    /// Indicates that the dependency is not required for compilation, but is for execution. It is in the runtime and test classpaths, but not the compile classpath.
    /// </summary>
    Runtime,

    /// <summary>
    /// Indicates that the dependency is not required for normal use of the application, and is only available for the test compilation and execution phases. It is not transitive.
    /// </summary>
    Test,

    /// <summary>
    /// Similar to <see cref="Provided"/> except that you have to provide the JAR which contains it explicitly. The artifact is always available and is not looked up in a repository.
    /// </summary>
    System
}