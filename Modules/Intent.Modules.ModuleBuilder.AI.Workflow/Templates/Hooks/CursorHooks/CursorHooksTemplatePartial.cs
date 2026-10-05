using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.CursorHooks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CursorHooksTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.CursorHooks";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CursorHooksTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string HarnessFolder = ".cursor";

        /// <summary>
        /// Cursor has its own hook schema - camelCase event names, a "version" integer, and a
        /// per-entry "failClosed" flag - rather than the "hooks.json" convention the generic template
        /// serves, which is why it is a template of its own. Every template is offered every
        /// AI.Context anchor, so landing anywhere but ".cursor" is the normal case and declines
        /// silently.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == HarnessFolder;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Written INSIDE the anchor. An escaped "../" path resolves identically from every
            // anchor, which is what previously collapsed several instances onto one file and stopped
            // the Software Factory dead.
            return new TemplateFileConfig(
                fileName: "hooks",
                fileExtension: "json",
                relativeLocation: ""
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Two corrections over the earlier generic-switch version, both from Cursor's own hooks
            // reference:
            //
            // 1. guard-write now runs on "preToolUse", NOT "afterFileEdit". Cursor has no
            //    before-file-edit hook, and afterFileEdit fires once the write has already landed, so
            //    it could never deny anything - the guard was advisory without saying so. preToolUse
            //    runs before ANY tool and CAN block. Its matcher takes Cursor's own tool categories -
            //    Shell, Read, Write, Grep, Delete, Task, and "MCP:<name>" - not another harness's
            //    tool names.
            //
            // 2. "failClosed": true is set on every blocking hook. Cursor otherwise treats any exit
            //    code that is not exactly 0 or 2 as ALLOW, so a crashed or missing gate would wave
            //    the action through. This is belt-and-braces with the wrapper in GateCommands.Guard,
            //    which already collapses non-zero exits to 2; the flag covers the cases the wrapper
            //    cannot, such as the command failing to start at all.
            //
            // sessionStart and stop are fire-and-forget by design and cannot block, so neither
            // carries failClosed.
            return $$"""
                {
                  "version": 1,
                  "hooks": {
                    "sessionStart": [
                      { "type": "command", "command": "{{GateCommands.Warm(HarnessFolder)}}" }
                    ],
                    "preToolUse": [
                      {
                        "type": "command",
                        "command": "{{GateCommands.Guard(HarnessFolder, "guard-write")}}",
                        "matcher": "Write|Delete",
                        "failClosed": true
                      }
                    ],
                    "beforeMCPExecution": [
                      {
                        "type": "command",
                        "command": "{{GateCommands.Guard(HarnessFolder, "guard-version")}}",
                        "matcher": ".*run_designer_script.*",
                        "failClosed": true
                      }
                    ],
                    "stop": [
                      { "type": "command", "command": "{{GateCommands.Guard(HarnessFolder, "close-out")}}" }
                    ]
                  }
                }
                """;
        }
    }
}