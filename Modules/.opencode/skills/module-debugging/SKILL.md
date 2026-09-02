---
name: module-debugging
description: "Insert temporary log statements into a designer script (console.log) or module C# code (templates, factory extensions) to see actual runtime values, then locate the resulting output. USE ONLY WHEN a designer script or a generated template isn't behaving as expected and the actual value is unknown, not guessed. DO NOT USE FOR permanent logging left in shipped module code — remove the temporary statements once the value is confirmed. REQUIRES the specific script or template already identified as the source of the unexpected behaviour."
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleDebugging_SkillMd_Agents
contentHash: 73BF4808F7DCABC1B1E558DD6BBB0B904EC941A24FCACE2F0F3CD97CD44CD1DC
---
# Module Debugging

Two separate mechanisms exist depending on where the code you're debugging runs — designer scripts (`run_designer_script`) and module C# code (templates, factory extensions, registrations). They are not interchangeable.

## Debugging a designer script

`run_designer_script` captures `console.log` / `console.warn` / `console.error` calls made inside the script and returns them in the result's output — this is your only visibility into a script's runtime values. Add a `console.log` wherever you need to confirm what a lookup returned, what a loop iterated over, or what a computed value actually is, then read the returned output after the call. Remove the log lines once you've confirmed the behaviour — they aren't meant to be permanent.

```js
const el = lookupByName("Customer");
console.log(`Customer id=${el?.getId()}, children=${el?.getChildren().length}`);
```

## Debugging module C# code (templates, factory extensions, registrations)

There is no interactive debugger available here — module code runs inside Intent Architect's own Software Factory process, not something you can attach a debugger to. The only way to see a runtime value is to write it to Intent's own log file, then read that file back.

### The logging API

```csharp
using Intent.Utils;

Logging.Log.Debug("your message");
Logging.Log.Info("your message");
Logging.Log.Warning("your message");
Logging.Log.Failure("your message");     // or Logging.Log.Failure(exception)
```

`Logging.Log` is an `Intent.Engine.ITracing`. Only these four levels exist — there is no `Error`, `Trace`, or `Fatal`, and no structured/templated overload (no `{Placeholder}` args) — build the full string yourself before passing it in.

### Where the output actually lands

Log files are **not** exposed through any MCP tool, environment variable, or config file — you have to know the convention and go find them yourself:

```
<OS temp directory>/IntentArchitect/logs/software-factory/sf-{contextId}-{pid}-{date}.log
```

- `<OS temp directory>` is whatever the OS's temp directory resolves to on the machine running Intent Architect — do not hardcode a path. Resolve it yourself: `echo %TEMP%` / `$env:TEMP` on Windows, `echo $TMPDIR` (falling back to `/tmp`) on macOS/Linux.
- The file is **day-rolling and per-process** (`{contextId}`/`{pid}` change every run) — there is no fixed filename. After running the Software Factory, list `logs/software-factory/` and pick the **most recently modified** `.log` file, not a guessed name.
- AI-driven module-task work (not a Software Factory run) logs to the sibling `logs/module-tasks/mt-{contextId}-{pid}-{date}.log` folder instead — check there if the code you're debugging runs as a module task rather than during SF generation.

### Make your own output easy to find

The log file also carries Intent Architect's own internal log lines — searching it blind is slow. Prefix every debug line you add with something distinctive and unlikely to collide (e.g. `[AI-DEBUG]`), then grep the log file for that exact prefix once the run completes:

```csharp
Logging.Log.Debug($"[AI-DEBUG] Customer.Name={model.Name}, Attributes={model.Attributes.Count}");
```

```bash
grep "\[AI-DEBUG\]" "<logs-folder>/software-factory/"*.log
```

### Workflow

1. Add one or more `Logging.Log.Debug("[AI-DEBUG] ...")` calls at the point you need visibility.
2. Rebuild the module and reinstall it / run the Software Factory on the target application.
3. Find the newest `.log` file in `logs/software-factory/` (or `logs/module-tasks/` for module-task code).
4. Grep it for your prefix to isolate just the lines you added.
5. Once you've confirmed the behaviour, remove the temporary log lines — they're for diagnosis, not permanent output.
