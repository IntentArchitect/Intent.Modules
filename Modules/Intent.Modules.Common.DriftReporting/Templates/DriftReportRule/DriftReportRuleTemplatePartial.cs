using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Common.DriftReporting.Templates.DriftReportRule
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DriftReportRuleTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Common.DriftReporting.DriftReportRule";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DriftReportRuleTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // The report's path names the application, so the text is built once the run's configuration is known.
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("drift-report", relativeLocation: "")
                .AfterBuild(file => file.FromMarkdown(Text(DriftReports.RelativePath(ExecutionContext))));
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

        [IntentManaged(Mode.Ignore)]
        internal static string Text(string reportPath)
        {
            return """
---
description: Every Software Factory run checks this application for drift between the model and the code. Read the drift report whenever a run reports drift.
alwaysApply: true
---

# Drift report

Modules installed in this application check it for drift on every Software Factory run: where the model and the code disagree, or where the code breaks a rule the model states.

## After a Software Factory run

- A run with drift **errors** fails with one error: how many errors and warnings each Area has, e.g. `Drift in Domain: 2 errors, 1 warning`, and where the report is.
- A run with only drift **warnings** ends with one warning in the same form, and doesn't fail. The results an agent gets may leave warnings out.
- **The report has every finding in full, warnings included:** `<<ReportPath>>`, in the `.cache` folder beside the solution's `.isln` file, which git ignores. A run with findings lists the report in its changes as a new file; a run with none writes no report. A run with errors applies none of its changes, so apply the report with `apply_staged_file_changes` and its path to read it from disk.
- A CI/CD run (`--param ensure-no-outstanding-changes=true`) never writes the report: its one error lists every finding instead.

## Reading the report

The report opens with a summary of the errors and warnings in each **Area**. An Area is one check, or several checks that share a name, such as Architecture or Domain. Each Area has a table per **category**:

- **Model / Code**: the model and the code disagree.
- **Code / Code**: code breaks a rule about other code, such as a layer importing one it may not depend on.
- **Code**: the code alone has a problem, such as a file no convention covers or a check's own configuration being wrong.

A row's **Subject** is what the finding is about (a model element or a file), **Against** is what it was compared with, and **Finding** says what is wrong. `✓ Clean` means the category was checked and nothing was found.

Never edit the report: every run rewrites it. Resolve each error, then run the Software Factory again until it reports no drift.
""".Replace("<<ReportPath>>", reportPath);
        }
    }
}
