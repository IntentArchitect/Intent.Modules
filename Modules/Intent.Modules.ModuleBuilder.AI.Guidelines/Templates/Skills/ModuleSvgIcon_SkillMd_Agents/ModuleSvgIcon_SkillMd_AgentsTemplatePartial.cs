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

namespace Intent.Modules.ModuleBuilder.AI.Guidelines.Templates.Skills.ModuleSvgIcon_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleSvgIcon_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Guidelines.Skills.ModuleSvgIcon_SkillMd_Agents";

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
                    keywords: [icon, svg, branding, design, imodspec, package, stereotype, element type, designer]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-svg-icon

                    Three things in a module can carry an icon, and each is set differently:

                    | What gets the icon | How it is set |
                    |---|---|
                    | The **module itself**, shown beside it in module lists | The bundled `scripts/set-module-icon.cs`, after the developer approves the image (Steps 1–6) |
                    | A **stereotype** you define | The Module Builder designer: the stereotype definition's icon, with `displayIcon` and `displayIconFunction` (see *Stereotype And Element-Type Icons*) |
                    | A **custom element type** you define (`Element Settings` / `Core Type`) | The Module Builder designer: `Icon`, `Expanded Icon` and `Icon Function` on its `Settings` stereotype (see *Stereotype And Element-Type Icons*) |

                    ## When to Use
                    To create or refresh a module's package icon, or to give a stereotype or element type its own icon. A
                    supplied description, from the developer or a workflow that sourced it, is used as given; when asked to
                    design the icon, follow Step 1's design steps. Apply a package icon only once the developer has approved
                    the rendered image (Step 3), and only at an unpublished version (Step 4).

                    ## The Bundled Script Sets The Package Icon
                    The package icon is not in the model, and no Intent Architect tool takes it from a file. Set it with the
                    bundled `scripts/set-module-icon.cs`, in this skill's folder, pointed at the module's folder (the one
                    holding its `.imodspec`). It is the only way this skill sets a package icon. The script writes the icon
                    where Intent Architect keeps it, and the Software Factory regenerates the `.imodspec`'s icon from that,
                    so a hand edit to the `.imodspec`'s icon is overwritten.

                    The script has three modes: set (`--svg`), check (`--verify`) and export the current icon (`--export`).
                    It prints only lengths, hashes and fixed messages, never the icon's encoded value, which runs to tens of
                    thousands of characters. If it stops with a message, report the message rather than working around it.
                    The script does not set stereotype or element-type icons.

                    ### Running The Script
                    It is a .NET file-based app, so it runs the same on any OS and needs only a .NET 10 SDK or later. Run it
                    with `dotnet run`, give every path as an absolute path, and put the script's own arguments after `--`.
                    Before its first real use, check it can run:

                    ```
                    dotnet run <this skill's folder>/scripts/set-module-icon.cs -- --check
                    ```

                    The first run builds the script, which can take a minute; later runs are quick. It prints
                    `set-module-icon is ready`. When it cannot run, the message comes from `dotnet`, not the script:

                    | Message | Cause | What to do |
                    |---|---|---|
                    | `dotnet` is not found or not recognized | No .NET SDK on `PATH` | Stop and ask the developer to install the .NET 10 SDK |
                    | `Couldn't find a project to run` | The SDK in use is older than .NET 10, often because a `global.json` in the working folder or above it pins one | Run `dotnet --list-sdks`. If it lists 10.0 or later, run the same command from a folder outside the repository, such as the OS temp folder. Otherwise stop and ask the developer to install the .NET 10 SDK |
                    | `A compatible .NET SDK was not found` | A `global.json` pins an SDK that is not installed | Run it from a folder outside the repository, as above |
                    | Build errors in `set-module-icon.cs` | The repository's build settings add something the script cannot build with | Copy the script to a new folder outside the repository and run the copy |

                    Never set the icon another way because the script cannot run: stop and tell the developer what failed.

                    ## Step 1 — Craft The SVG
                    Skip this step when a built-in icon fits a stereotype or element type (see *Setting A Built-In Icon*).
                    Write drafts to an OS temp folder by shell command, never a project path, and keep the approved one
                    until Step 6.

                    ### Designing (skip when a description was supplied)
                    1. Export the current package icon with the script (command below) and render it. A placeholder (often
                    the Intent Architect logo) can go; ask before replacing a deliberate icon.
                    2. Render the neighbours: the library this module is most likely confused with, and its own family.
                    3. Draft three or four concepts as real SVGs and compare them rendered, not described.
                    4. Choose what the module *is*, not only what is special about it: the diagram someone would draw of
                    its idea. A hub joined to parts that only connect through it says "mediator"; code brackets say
                    "generated code".
                    5. Avoid shapes that already mean something (three dots in a V is "share", `</>` is code, a diamond
                    is "decision"), and a monogram when a library it is compared with uses one.

                    To export the current icon for item 1:

                    ```
                    dotnet run <this skill's folder>/scripts/set-module-icon.cs -- --module <module folder> --export <temp folder>/current.svg
                    ```

                    The script says if the icon is a PNG (it writes a `.png` instead) or a built-in icon (nothing to export).

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

                    ## Step 4 — Apply With The Bundled Script
                    The module must be at an unpublished version. If `module-version-increment` is available, its gate
                    decides; load it rather than judging from release notes or history, which never show what is
                    published. Without it, apply the icon and tell the developer which version it is in, so they can
                    confirm that version is unpublished. A further iteration within that version needs no further
                    increment.

                    Apply the approved SVG with the bundled script:

                    ```
                    dotnet run <this skill's folder>/scripts/set-module-icon.cs -- --module <module folder> --svg <approved icon>.svg
                    ```

                    The script changes nothing if the module's icon setting is not in the shape it expects; if it stops,
                    report its message rather than working around it.

                    ## Step 5 — Regenerate
                    Run the Software Factory. In Staging mode, pass `autoApply: false` and apply only the module's
                    `.imodspec` with `apply_staged_file_changes`. When Intent writes the output straight to disk instead,
                    nothing can be applied selectively. Either way, tell the developer about any change beyond the icon.
                    Check the change with `git diff --stat` only, since a full diff carries the encoded icon, and let Step
                    6's hashes prove the value.

                    ## Step 6 — Verify By Hash
                    Build with `dotnet build --no-incremental` and note the `.imod` path it reports. Run the same script with
                    `--verify`:

                    ```
                    dotnet run <this skill's folder>/scripts/set-module-icon.cs -- --module <module folder> --svg <approved icon>.svg --verify --imod <the .imod path>
                    ```

                    It prints only the hashes of the icon built from the SVG, the icon it stored, the module's `.imodspec`
                    icon and the icon packaged in the `.imod`, and exits non-zero unless all four match. Then delete the
                    drafts, exports, renders and browser profile.

                    ## Stereotype And Element-Type Icons

                    These icons are set in the Module Builder designer and generated into the module's `.designer.settings`
                    and packaged stereotype definitions. The bundled script plays no part here.

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
                    - [ ] Package icon: set with the bundled `set-module-icon.cs --module`, at an unpublished version
                    - [ ] Package icon: only the `.imodspec` applied from the Software Factory run
                    - [ ] Package icon: `set-module-icon.cs --verify --imod` reports all four hashes match; temp files removed
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
