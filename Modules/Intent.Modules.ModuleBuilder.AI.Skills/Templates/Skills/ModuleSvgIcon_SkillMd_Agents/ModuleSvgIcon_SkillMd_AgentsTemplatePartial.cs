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
                    description: "Design or craft a house-style SVG icon for a module and apply it: as the module's package icon, after rendering it at the sizes Intent Architect shows it and getting the developer's approval on the image, or as a built-in or bespoke icon on a stereotype definition or custom element type the module defines. USE ONLY WHEN asked to create, design, set or refresh a module's own package icon, or to give a stereotype or element type its own icon. DO NOT USE FOR icons in a generated application's own UI or web assets. REQUIRES a description of what the icon should represent, or the developer's request that you design one; for a stereotype or element type, that element already modelled in the Module Builder designer."
                    argument-hint: "[what gets the icon] [what it should represent, or leave blank to have it designed]"
                    keywords: [icon, svg, branding, design, application.config, imodspec, package, stereotype, element type, designer]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-svg-icon

                    Three things in a module can carry an icon, and each is stored and set somewhere different:

                    | What gets the icon | Where it lives | How it is set |
                    |---|---|---|
                    | The **module itself**, shown beside it in module lists | The module's `.application.config`: a root-level, attribute-less `<icon>` with a sibling `<iconType>` | Steps 1–6 below, applied by script |
                    | A **stereotype** you define | Fields on the stereotype definition: an `icon` (a type + source pair), plus `displayIcon` and `displayIconFunction` | The Module Builder designer — see *Stereotype And Element-Type Icons* |
                    | A **custom element type** you define (`Element Settings` / `Core Type`) | The `Settings` stereotype applied to it: `Icon`, `Expanded Icon`, `Icon Function` | The Module Builder designer — see *Stereotype And Element-Type Icons* |

                    ## When to Use
                    To create or refresh a module's package icon, or to give a stereotype or element type its own icon. A
                    supplied description, from the developer or a workflow that sourced it, is used as given; when asked to
                    design the icon, follow Step 1's design steps. Apply a package icon only once the developer has approved
                    the rendered image (Step 3).

                    ## Where The Package Icon Lives
                    Not in the model: neither `Module Settings` nor the package has an `Icon` field. It is the root-level,
                    attribute-less `<icon>data:image/svg+xml;base64,...</icon>` in the module's `.application.config`, with a
                    sibling `<iconType>UrlImagePath</iconType>`. The Software Factory generates `.imodspec`'s `<iconUrl>`
                    from it, so a hand edit to `<iconUrl>` is discarded, the same trap `module-versioning` describes for
                    `<version>`.

                    ## The One Approved Exception
                    `.application.config` is off-limits, and the package icon is the one exception to that, and to any rule
                    to change files only with edit tools (where hooks check edits, they deny this file anyway). The reason is
                    the payload, not a missing mechanism: `update_application_settings` does take `icon`, but the base64
                    value runs to tens of thousands of characters, and putting it through your context is wasteful and risks
                    silent corruption. So:

                    - Change only the root `<icon>`/`<iconType>` pair, by script. Leave every `<icon type="..." source="...">`
                    entry (designer references, settings groups) and every other value alone.
                    - Never open, read or print the file, even in part, and never read or reproduce the base64 value: the
                    script computes and writes it. The root `<icon>` line starts with the payload, so `cat`, `head`, `cut`,
                    `Select-String`, a diff or any command that prints a line of the file leaks it. You never need to look:
                    the Step 4 script checks the file's shape itself. If you want to check it first, print counts only, such
                    as `grep -c '<icon>' <file>`. The `.imodspec`'s `<iconUrl>` holds the same value, so read that file only with the line
                    filtered out (`grep -v iconUrl`).
                    - Everything else, such as release notes or recorded decisions, is edited with your edit tools as usual.

                    Stereotype and element-type icons never live in `.application.config`, so the exception does not cover them.

                    ## Step 1 — Craft The SVG
                    Skip this step when a built-in icon fits a stereotype or element type (see *Setting A Built-In Icon*).
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

                    An element or stereotype icon renders smaller than a package icon and sits in a dense tree, so the
                    legibility rules bind harder there, not less. A bespoke one that ignores the house style is more jarring
                    than a generic built-in one.

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
                    The module must be at an unpublished version. If `module-version-increment` is available, its gate
                    decides; load it rather than judging from release notes or history, which never show what is
                    published. Without it, apply the icon and tell the developer which version it is in, so they can
                    confirm that version is unpublished. A further iteration within that version needs no further
                    increment. Run this PowerShell script with the
                    two paths filled in, without reading the config first. It base64-encodes the approved SVG, asserts
                    exactly one attribute-less root `<icon>` and at most one `<iconType>` (adding it if absent), sets them,
                    keeps the file's encoding, and prints only the value's length. A Bash script must do the same.

                    ```powershell
                    $config = '<module folder>/<Module>.application.config'
                    $svg    = '<approved icon>.svg'
                    $uri   = 'data:image/svg+xml;base64,' + [Convert]::ToBase64String([IO.File]::ReadAllBytes($svg))
                    $bytes = [IO.File]::ReadAllBytes($config)
                    $bom   = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
                    $text  = [IO.File]::ReadAllText($config)
                    $icon  = [regex]::Matches($text, '<icon>[^<]*</icon>')
                    $type  = [regex]::Matches($text, '<iconType>[^<]*</iconType>')
                    if ($icon.Count -ne 1 -or $type.Count -gt 1) { throw "Expected one root <icon> and at most one <iconType>, found $($icon.Count) and $($type.Count)" }
                    $text = $text.Replace($icon[0].Value, "<icon>$uri</icon>")
                    if ($type.Count -eq 1) { $text = $text.Replace($type[0].Value, '<iconType>UrlImagePath</iconType>') }
                    else { $text = $text.Replace("<icon>$uri</icon>", "<icon>$uri</icon>`n  <iconType>UrlImagePath</iconType>") }
                    [IO.File]::WriteAllText($config, $text, (New-Object System.Text.UTF8Encoding $bom))
                    "Set the root <icon> ($($uri.Length) characters) and <iconType>."
                    ```

                    ## Step 5 — Regenerate
                    Run the Software Factory. In Staging mode, pass `autoApply: false` and apply only the module's
                    `.imodspec` with `apply_staged_file_changes`. When Intent writes the output straight to disk instead,
                    nothing can be applied selectively. Either way, tell the developer about any change beyond the icon.
                    Never diff or read the `.imodspec` or `.application.config` to check the result: `get_file_diffs`,
                    `git diff` and reading either file all print the new value. Confirm by counts only, with
                    `git diff --stat` on the two files (one or two lines each), and let Step 6's hashes prove the value.

                    ## Step 6 — Verify By Hash
                    Build with `dotnet build --no-incremental` and note the `.imod` path it reports. In one script, print
                    only the hashes of: the data URI from the approved SVG, the root `<icon>` in `.application.config`, the
                    repository `.imodspec`'s `<iconUrl>`, and the `<iconUrl>` in the `.imodspec` inside the `.imod` (a zip).
                    All four must match. An error inside that script can echo its arguments, payload included: in PowerShell,
                    give a helper a verb-noun name, because a short one such as `H` is a built-in alias (`Get-History`), and
                    its error prints the whole data URI. Then delete the drafts, renders and browser profile.

                    ## Stereotype And Element-Type Icons

                    These icons are set in the Module Builder designer and generated into the module's `.designer.settings`
                    and packaged stereotype definitions. They never live in `.application.config`, so nothing in this section
                    reads or edits that file.

                    ### Stereotype Icons

                    A stereotype's icon is **structural** — part of the stereotype definition, not a property you apply to it.
                    It is a type/source pair: a `FontAwesome` type takes an icon name as its source (`cogs`, `database`); a
                    `UrlImagePath` type takes a data URI.

                    Two companions control *when* it shows:

                    - **`displayIcon`** — whether the badge renders on elements carrying the stereotype at all.
                    - **`displayIconFunction`** — a script for conditional display, when the badge should only appear for
                      certain property values. Prefer plain `displayIcon` unless the condition is real; a function that always
                      returns the same answer is just a slower checkbox.

                    A stereotype badge competes for space with the element's own icon and its name. Reach for one when the
                    stereotype changes how the element should be *read* at a glance — not merely to show it was applied.

                    ### Element Type Icons

                    For an element type you define — an `Element Settings` or `Core Type` node — the icon is a property on the
                    `Settings` stereotype that node carries:

                    | Property | Purpose |
                    |---|---|
                    | `Icon` | The element's icon in the model tree and on diagrams |
                    | `Expanded Icon` | Optional alternate shown when the node is expanded. Leave unset unless expanding genuinely changes what the node represents. |
                    | `Icon Function` | A script returning an icon per element, for types whose icon depends on their own state. **Returning null falls back to the default icon** — that is the designed escape, not a failure. |

                    `Icon` and `Icon Function` are alternatives. Set the static `Icon` unless the icon genuinely varies per
                    element; a function is re-evaluated as the model changes and is harder to reason about.

                    ### Setting A Built-In Icon

                    For anything the shipped icon set already covers, use the designer's icon picker — or, for a stereotype,
                    a FontAwesome name. Both store a short reference, and there is nothing to hand-author:

                    | Target | Stored as |
                    |---|---|
                    | Stereotype | `<icon type="FontAwesome" source="cogs" />` on the definition |
                    | Element type | The `Icon` property, as JSON: `{"type":0,"source":"./img/icons/…"}` |

                    ### Setting A Bespoke SVG Icon

                    When the built-in set does not carry the concept, craft the SVG (Step 1) and render it (Step 2). The icon
                    becomes a base64 data URI in exactly the same slot — the `source` stops being a path and becomes the
                    encoded image:

                    | Target | Stored as |
                    |---|---|
                    | Stereotype | `<icon type="UrlImagePath" source="data:image/svg+xml;base64,…" />` |
                    | Element type | `Icon` property JSON with `"source":"data:image/svg+xml;base64,…"` |

                    The value is a designer property, so it is set with `run_designer_script` — not by editing designer
                    metadata files. Never read or reproduce the base64 yourself: have a shell script compute it and write a
                    small script file that assigns it, then pass that file to `run_designer_script` via `includedScriptPaths`
                    so it is prepended to your own script. You author the logic; the generated file carries the string. To
                    reuse one icon on several elements, set each from the same source SVG rather than copying an encoded
                    value between them: copying is where silent truncation happens.

                    Then run the Software Factory. It propagates the value into the module's generated `.designer.settings`,
                    which is what consuming applications actually read — the designer model is the source, that file is the
                    output.

                    ## Checklist
                    - [ ] Icon set on the right thing: the package icon, a stereotype definition, or an element type's `Settings` stereotype
                    - [ ] Supplied description used as given, or the icon designed through Step 1
                    - [ ] A built-in icon used for a stereotype or element type wherever one fits; a bespoke SVG only where the concept is not covered
                    - [ ] House style and legibility rules measured and met, or a deviation stated
                    - [ ] Base64 never read, echoed or copied by hand — computed by script every time
                    - [ ] Package icon: rendered at 16–128px on light and dark, with a pixel zoom and a module-list mock-up
                    - [ ] Package icon: the developer approved the rendered image before anything was applied
                    - [ ] Package icon: only the root `<icon>`/`<iconType>` changed, by script, counts asserted, file never read
                    - [ ] Package icon: only the `.imodspec` applied from the Software Factory run
                    - [ ] Package icon: all four hashes match; temp files removed
                    - [ ] Element icon: `Icon` used rather than `Icon Function`, and `Expanded Icon` left unset, unless the icon genuinely varies
                    - [ ] Stereotype or element icon: set through `run_designer_script`, the Software Factory run, and the icon confirmed to render in the designer
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
