using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_ResourcesDebuggingMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileBuilderExpert_ResourcesDebuggingMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.FileBuilderExpert_ResourcesDebuggingMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FileBuilderExpert_ResourcesDebuggingMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("debugging", relativeLocation: "file-builder-expert/resources")
                .FromMarkdown(""""""
                    # Debugging Module Code: Seeing A Runtime Value

                    When a template, factory extension, registration or module task misbehaves and you need the actual
                    value of something at run time, log it. There is no debugger to attach: module code runs inside Intent
                    Architect's own Software Factory process. (A designer script is different: `run_designer_script` returns
                    its `console.log` output directly.)

                    ===

                    ## Logging A Value

                    ```csharp
                    using Intent.Utils;

                    Logging.Log.Debug($"[AI-DEBUG] Customer.Name={model.Name}, Attributes={model.Attributes.Count}");
                    ```

                    `Logging.Log` is an `Intent.Engine.ITracing` with four levels only — `Debug`, `Info`, `Warning` and
                    `Failure` (which also takes an exception). There is no `Error`, `Trace` or `Fatal`, and no templated
                    `{Placeholder}` overload, so build the whole string yourself.

                    ===

                    ## Where The Output Lands

                    The output is not exposed through any tool, environment variable or config file. It lands in
                    `<OS temp directory>/IntentArchitect/logs/software-factory/sf-{contextId}-{pid}-{date}.log`:

                    - Resolve the temp directory on the machine running Intent Architect (`$env:TEMP` on Windows, `$TMPDIR`
                      falling back to `/tmp` on macOS and Linux); never hardcode it.
                    - The file is day-rolling and per process, so there is no fixed name. After the run, pick the most
                      recently modified `.log` file in that folder.
                    - Code that runs as a module task rather than during a Software Factory run logs to the sibling
                      `logs/module-tasks/` folder instead.
                    - The file also carries Intent Architect's own log lines, so prefix yours with something distinctive
                      (`[AI-DEBUG]`) and search for it.

                    ===

                    ## Getting The Line To Appear, Then Removing It

                    Rebuild the module with `dotnet build --no-incremental` and confirm the reinstall picked it up (the
                    `known-build-gotchas` instructions, "Template Changes Not Taking Effect"); without that, the previously
                    packaged module runs and your line never appears. Then run the Software Factory on the target
                    application, read the value, and remove the temporary log lines — they are for diagnosis, not shipped
                    output.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
