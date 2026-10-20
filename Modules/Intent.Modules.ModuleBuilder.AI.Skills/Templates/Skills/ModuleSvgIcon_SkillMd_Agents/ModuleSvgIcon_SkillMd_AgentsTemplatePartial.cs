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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.ModuleSvgIcon_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleSvgIcon_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.ModuleSvgIcon_SkillMd_Agents";

        internal const string SkillName = "module-svg-icon";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleSvgIcon_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Design or craft a house-style SVG icon for a module, render it at the sizes Intent Architect shows it, get the developer's approval on the rendered image, and apply it as the module's package icon. USE ONLY WHEN asked to create, design, set, or refresh a module's own package icon. DO NOT USE FOR an icon on a stereotype or a custom element type (see module-element-icons). REQUIRES a description of what the icon should represent, or the developer's request that you design one."
                    argument-hint: "[what the icon should represent, or leave blank to have it designed]"
                    keywords: [icon, svg, branding, design, application.config, imodspec, package]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-svg-icon

                    ## When to Use
                    To create or refresh a module's package icon, the one shown beside it in module lists. A supplied
                    description, from the developer or a workflow that sourced it, is used as given; when asked to design
                    the icon, follow Step 1's design steps. Either way, apply it only once the developer has approved the
                    rendered image (Step 3). For a stereotype or element-type icon, use `module-element-icons`.

                    ## Where The Icon Lives
                    Not in the model: neither `Module Settings` nor the package has an `Icon` field. It is the root-level,
                    attribute-less `<icon>data:image/svg+xml;base64,...</icon>` in the module's `.application.config`, with a
                    sibling `<iconType>UrlImagePath</iconType>`. The Software Factory generates `.imodspec`'s `<iconUrl>`
                    from it, so a hand edit to `<iconUrl>` is discarded, the same trap `module-versioning` describes for
                    `<version>`.

                    ## The One Approved Exception
                    `.application.config` is off-limits, and this skill is the one exception to that, and to any rule to
                    change files only with edit tools (where hooks check edits, they deny this file anyway). The reason is the
                    payload, not a missing mechanism: `update_application_settings` does take `icon`, but the base64 value
                    runs to tens of thousands of characters, and putting it through your context is wasteful and risks
                    silent corruption. So:

                    - Change only the root `<icon>`/`<iconType>` pair, by script. Leave every `<icon type="..." source="...">`
                    entry (designer references, settings groups) and every other value alone.
                    - Never open, read or print the file, and never read or reproduce the base64 value: the script computes
                    and writes it. The `.imodspec`'s `<iconUrl>` holds the same value, so read that file only with the line
                    filtered out (`grep -v iconUrl`).
                    - Everything else, such as release notes or recorded decisions, is edited with your edit tools as usual.

                    ## Step 1 — Craft The SVG
                    Write drafts to an OS temp folder by shell command, never a project path, and keep the approved one
                    until Step 6.

                    ### Designing (skip when a description was supplied)
                    1. Decode the current `<iconUrl>` to a temp `.svg` by script and render it. A placeholder (often the
                    Intent Architect logo) can go; ask before replacing a deliberate icon.
                    2. Render the neighbours: the library this module is most likely confused with, and its own family.
                    3. Draft three or four concepts as real SVGs and compare them rendered, not described.
                    4. Choose what the module *is*, not only what is special about it: the diagram someone would draw of
                    its idea. A hub joined to parts that only connect through it says "mediator"; code brackets say
                    "generated code".
                    5. Avoid shapes that already mean something (three dots in a V is "share", `</>` is code, a diamond
                    is "decision"), and a monogram when a library it is compared with uses one.

                    A family of modules may share one icon by design. Record that where the module keeps its decisions.

                    ### House style
                    Deviate only for a stated reason.

                    | Aspect | Default |
                    |---|---|
                    | Canvas | `viewBox="0 0 50 50"`, square |
                    | Badge | Full-bleed circle: `<circle cx="25" cy="25" r="24.4">` |
                    | Fill | A 2–4 stop linear gradient in one hue family (flat is an acceptable fallback) |
                    | Glyph | One simple white (`fill="#fff"`) line-art or solid shape, centered, a handful of paths at most |
                    | Accent | Optional: one small detail in a darker shade of the gradient's hue. No other hues, shadows or photographic detail |
                    | Text | Avoid; a 1–2 letter monogram only if nothing else reads |

                    ### Legibility at 16px
                    Measure, don't eyeball. At 16px one pixel is 3.125 canvas units (`units × 16 / 50`).

                    - Gaps of at least about 1px (about 3 units); smaller ones blur shapes together.
                    - No detail under about 4px; a smaller one becomes a stray dot.
                    - Glyph about 55–60% of the badge's height, and at least about 6 units clear of its edge.
                    - Fewer, larger parts: four spokes stay crisp where six look like a snowflake. Simplify rather than add detail.
                    - White glyph at 3:1 WCAG contrast or better against each gradient stop it sits over: white on `#2dd4bf`
                    is 1.86:1, on `#0d9488` 3.74:1. Darken stops rather than thin the glyph. Check the darkest stop also
                    shows against a dark background (`#1e1e1e`). Ratio: `(L1 + 0.05) / (L2 + 0.05)` on relative luminance.

                    ### Detail that depends on size
                    Hide a detail that blots small renders with a media query inside the SVG. An image evaluates it against
                    its own rendered size in Chromium, which Intent Architect uses; a renderer that ignores it shows the
                    full detail.

                    ```xml
                    <style>@media (max-width: 30px) { .bolt { display: none; } }</style>
                    ```

                    ## Step 2 — Render And Review
                    Never judge an icon from its source. Screenshot an HTML sheet of `<img>` tags with headless Chromium
                    (Chrome, Edge or Chromium), using a throwaway profile:

                    ```
                    <browser> --headless=new --disable-gpu --hide-scrollbars --force-device-scale-factor=1 --window-size=720,560 --user-data-dir=<temp profile> --screenshot=<out.png> file:///<sheet.html>
                    ```

                    - **Size ramp:** each concept at 16, 20, 24, 32, 48, 64 and 128px on light and dark, neighbours alongside.
                    - **Module-list mock-up:** icon and module name at 20 and 32px beside the neighbours.
                    - **Pixel zoom:** in the same sheet, draw the icon onto a canvas at 16, 20 and 24px and show it about 5×
                    with `image-rendering: pixelated`, which shows the real pixels a vector zoom hides:

                    ```html
                    <canvas id="z16" width="16" height="16" style="width:80px;height:80px;image-rendering:pixelated"></canvas>
                    <script>
                      const i = new Image();
                      i.onload = () => document.getElementById('z16').getContext('2d').drawImage(i, 0, 0, 16, 16);
                      i.src = 'icon.svg';
                    </script>
                    ```

                    Look at every render yourself and measure it against Step 1.

                    On Windows, Edge is `msedge.exe` under `C:\Program Files (x86)\Microsoft\Edge\Application\`; from
                    PowerShell, launch it with `Start-Process -Wait` so the screenshot exists before you read it.

                    ## Step 3 — Get Approval On The Image
                    Show the developer the rendered sheets themselves: open or attach them. A file path, or a concept picked
                    from a list of descriptions, is not approval. Ask whether to apply it as it is, change something, or
                    not apply it yet, and apply nothing until the answer is "apply". This holds when another workflow
                    called this skill; with no one to approve, leave the icon unapplied and report where the renders are.

                    ## Step 4 — Apply By Script
                    The module must be at an unpublished version (`module-version-increment` decides, if available); a
                    further iteration within that version needs no further increment. In one PowerShell or Bash script:

                    1. Base64 the approved `.svg` file's bytes and build `data:image/svg+xml;base64,<value>`.
                    2. In `.application.config`, assert exactly one attribute-less root `<icon>` and exactly one
                    `<iconType>` (add `<iconType>` beside `<icon>` if it is absent). Abort on any other count.
                    3. Set `<icon>` to the data URI and `<iconType>` to `UrlImagePath`.

                    ## Step 5 — Regenerate
                    Run the Software Factory. In Staging mode, pass `autoApply: false` and apply only the module's
                    `.imodspec` with `apply_staged_file_changes`. When Intent writes the output straight to disk instead,
                    nothing can be applied selectively. Either way, tell the developer about any change beyond the icon.
                    Confirm with `get_file_diffs` that only the `.imodspec`'s `<iconUrl>` changed, and with
                    `git diff --stat` that `.application.config` changed by one or two lines, never by reading the base64.

                    ## Step 6 — Verify By Hash
                    Build with `dotnet build --no-incremental` and note the `.imod` path it reports. In one script, print
                    only the hashes of: the data URI from the approved SVG, the root `<icon>` in `.application.config`, the
                    repository `.imodspec`'s `<iconUrl>`, and the `<iconUrl>` in the `.imodspec` inside the `.imod` (a zip).
                    All four must match. An error inside that script can echo its arguments, payload included: in PowerShell,
                    give a helper a verb-noun name, because a short one such as `H` is a built-in alias (`Get-History`), and
                    its error prints the whole data URI. Then delete the drafts, renders and browser profile.

                    ## Checklist
                    - [ ] Supplied description used as given, or the icon designed through Step 1
                    - [ ] House style and legibility rules measured and met, or a deviation stated
                    - [ ] Rendered at 16–128px on light and dark, with a pixel zoom and a module-list mock-up
                    - [ ] The developer approved the rendered image before anything was applied
                    - [ ] Only the root `<icon>`/`<iconType>` changed, by script, counts asserted, file and base64 never read
                    - [ ] Only the `.imodspec` applied from the Software Factory run
                    - [ ] All four hashes match; temp files removed
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
