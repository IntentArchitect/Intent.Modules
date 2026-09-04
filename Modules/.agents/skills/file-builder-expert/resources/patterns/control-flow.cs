// PATTERN: Control Flow & Statements
// ===================================
// KEY RULES:
//   - else, else-if, catch, finally are SIBLING statements on the METHOD — never children of the if/try block.
//   - All block types (if, foreach, using, try) default BeforeSeparator = EmptyLines (blank line before).
//   - else / catch / finally default BeforeSeparator = NewLine (no blank line — they cluster with the preceding block).
//   - Braces and indentation are automatic. Never write { } manually.
//   - CSharpMethodChainStatement is [Obsolete]. Use CSharpInvocationStatement instead.

// --- IF / ELSE IF / ELSE ---

method.AddIfStatement("value == 0", @if =>
{
    @if.AddStatement("throw new ArgumentException(\"Zero not allowed\");");
});
method.AddElseIfStatement("value < 0", @elseIf =>   // sibling on method, not child of if
{
    @elseIf.AddStatement("value = 0;");
});
method.AddElseStatement(@else =>                      // sibling on method, not child of else-if
{
    @else.AddStatement("DoWork(value);");
});

// Multi-line condition: GetFormattedMultilineText auto-aligns continuation lines.
method.AddIfStatement(@"
    !string.IsNullOrWhiteSpace(configuration[""Key""]) &&
    !string.IsNullOrWhiteSpace(configuration[""Secret""])", block =>
{
    block.AddStatement("Configure();");
});

// Suppress the automatic blank line before this block:
method.AddIfStatement("x > 0", block => block
    .AddStatement("Process(x);")
    .SeparatedFromPrevious(false));

// --- FOR EACH ---

method.AddForEachStatement("item", "items", loop =>
{
    loop.AddStatement("Process(item);");
});

// Async enumeration — use the constructor directly and call .Await():
method.AddStatement(new CSharpForEachStatement("item", "GetItemsAsync()").Await());

// --- COMPLEX LINQ: BUILDER VS RAW STRING ---
// Prefer builder blocks for lambda arrows and nested invocation composition.

// GOOD (builder blocks):
method.AddStatement(new CSharpStatement("items")
    .AddInvocation("Where", where => where
        .AddArgument(new CSharpLambdaBlock("x")
            .WithExpressionBody(new CSharpStatement("x.IsActive && x.Score > threshold"))))
    .AddInvocation("Select", select => select
        .AddArgument(new CSharpLambdaBlock("x")
            .WithExpressionBody(new CSharpObjectInitializerBlock("new ResultDto")
                .AddInitStatement("Id", "x.Id")
                .AddInitStatement("Name", "x.Name"))))
    .AddInvocation("ToList", done => done.OnNewLine()));

// BAD (raw string interpolation):
// method.AddStatement($"items.Where(x => x.IsActive && x.Score > {threshold}).Select(x => new ResultDto {{ Id = x.Id, Name = x.Name }}).ToList();");
// Do not handwrite lambda arrows (=>) or object initializer braces ({ }) when builder blocks exist.

// --- WHILE ---

method.AddStatement("var done = false;");
method.AddWhileStatement("!done", loop =>
{
    loop.AddStatement("done = ProcessNext();");
});

// --- USING BLOCK ---
// NOTE:
//   - AddUsingBlock(...) creates a method-body using statement.
//   - File-level namespace imports are added with CSharpFile.AddUsing(...) or template.AddUsing(...).

method.AddUsingBlock("var scope = serviceProvider.CreateScope()", block =>
{
    block.AddStatement("var svc = scope.ServiceProvider.GetRequiredService<IMyService>();");
    block.AddStatement("await svc.ExecuteAsync();");
});

// --- TRY / CATCH / FINALLY ---
// catch and finally are SIBLINGS on the method, not children of try.

method.AddTryBlock(@try =>
{
    @try.AddStatement("await DoRiskyWork();");
});
method.AddCatchBlock("OperationCanceledException", "ex", @catch =>  // typed catch with parameter
{
    @catch.AddStatement("logger.LogWarning(ex, \"Cancelled\");");
});
method.AddCatchBlock(@catch =>                                        // bare catch-all
{
    @catch.AddStatement("// fallback handler");
});
method.AddFinallyBlock(@finally =>                                    // finally is a sibling
{
    @finally.AddStatement("Cleanup();");
});

// catch when clause — use the fluent modifiers on CSharpCatchBlock:
method.AddCatchBlock(@catch =>
{
    @catch.WithExceptionType("Exception")
          .WithParameterName("e")
          .WithWhenExpression("e.Message == \"retryable\"");
    @catch.AddStatement("Retry();");
});

// --- ASSIGNMENTS ---

// Convenience extension (preferred for variable declarations):
method.AddAssignmentStatement("var result", new CSharpStatement("await service.GetAsync()"));

// Direct constructor (use when both sides are complex statements):
method.AddStatement(new CSharpAssignmentStatement(
    new CSharpVariableDeclaration("var result"),
    new CSharpStatement("await service.GetAsync()")).WithSemicolon());

// --- INVOCATIONS & FLUENT CHAINS ---
// Do NOT use CSharpMethodChainStatement — it is [Obsolete].
// Use CSharpInvocationStatement (AddInvocationStatement / .AddInvocation() chain) instead.

// Simple call:
method.AddInvocationStatement("service.Execute", stmt => stmt
    .AddArgument("request")
    .AddArgument("cancellationToken"));

// Multi-arg each on its own line:
method.AddInvocationStatement("builder.Configure", stmt => stmt
    .WithArgumentsOnNewLines()
    .AddArgument("\"ConnectionString\"")
    .AddArgument("options => options.UseRetry()"));

// Fluent chain — .OnNewLine() forces each call onto a new indented line:
method.AddStatement(new CSharpStatement("service")
    .AddInvocation("MethodOne")
    .AddInvocation("MethodTwo", s => s.OnNewLine())
    .AddInvocation("MethodThree", s => s.OnNewLine()));

// Deeply nested lambda inside a multi-step invocation:
method.AddInvocationStatement("services.AddOpenTelemetry", main => main
    .AddInvocation("ConfigureResource", inv => inv
        .AddArgument(new CSharpLambdaBlock("res")
            .WithExpressionBody(new CSharpStatement("res")
                .AddInvocation("AddService", i => i.AddArgument("\"MyService\"").OnNewLine())
                .AddInvocation("AddTelemetrySdk", i => i.OnNewLine())
                .WithoutSemicolon()))));
