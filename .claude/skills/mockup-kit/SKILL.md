---
description: Build design-system-accurate HTML mockups of any web app for a spec, and show them in the spec's documents with the `<Mockup>` block. Covers snapshotting the app's real built CSS, fonts, images and shell markup into `intent/.mockups/<app>/` (a developer task, once per app), then building before/after screens under `intent/.specs/<slug>/mockups/` with pins that cite requirement ids, and embedding them. Use when a requirement changes what a user sees and a `<Wireframe>` sketch is not faithful enough, or when asked to mock up, prototype the look of, or snapshot the styles of a screen.
short-description: Snapshot a web app's real styles once, then build pinned before/after HTML mockups for a spec and embed them with <Mockup>.
intent-updates: automatic
requiredTools:
  - read_file
  - glob
  - grep
  - write_file
  - patch_file
  - write_spec
  - open_preview
  - run_preview_script
group: Documents, preview and tasks
---

# Mockup kit

A **mockup** is a static HTML page that links the app's own compiled CSS, so it looks exactly like the product in both themes. It is built from the app's real markup, annotated with numbered pins that cite requirement ids, and shown in a spec's document with the `<Mockup>` block. No framework, no build.

There are two jobs, done by different people:

| Job                                     | Who                                             | Where                                    | How often                                 |
| --------------------------------------- | ----------------------------------------------- | ---------------------------------------- | ----------------------------------------- |
| **Snapshot** the app's styles and shell | A developer (needs a shell and the app's build) | `intent/.mockups/<app>/`                 | Once per app; again when its look changes |
| **Mock up** a screen for a spec         | Anyone                                          | `intent/.specs/<slug>/mockups/<screen>/` | Per spec                                  |

**What to commit.** Always the recipe, `snapshot.json` and `templates/`, and every spec's `mockups/`. The script's output (`styles.css`, `kit/`, `SOURCE.md`) is the team's choice:

- **Everyone who opens a spec can build the app**: gitignore the output (`/intent/.mockups/*/styles.css`, `/intent/.mockups/*/kit/`, `/intent/.mockups/*/SOURCE.md`). A `<Mockup>` whose snapshot is not built shows a **Build snapshot** button that runs the script (and the app's `prepare` when its build output is missing), then reloads. If you find it unbuilt and can run commands, build it yourself before Part 2: `node <skill>/scripts/snapshot.mjs intent/.mockups/<app>/snapshot.json --prepare-if-needed`.
- **Readers can't build it** (no toolchain, or a clone without the app's source): commit the output too, and refresh it when the app's look changes.

Mocking up never takes or edits a snapshot; if there is no `snapshot.json` for the app, say so and ask a developer to run Part 1.

## Part 1: snapshot an app (developer)

The snapshot is everything a mockup needs to look like the app: one self-contained `styles.css` made from the app's **build output**, plus a hand-built shell.

1. **Find the built CSS, fonts and images.** Read the app's entry HTML (`index.html`, a `*-index.html`, a server layout) to see which stylesheets it loads and in what order. Prefer build output (`dist/`, `build/`, `wwwroot/css/*.css`, `.next/static/css`) over sources: LESS, SCSS, Tailwind and CSS-in-JS only become real CSS after a build. If the CSS only exists inside a running dev server (injected by JS), run the production build and use its output. Note where fonts (`@font-face` `url()`s) and icon fonts come from, often `node_modules`.
2. **Find how dark mode is switched**: a `data-theme` attribute, a `dark` class on `<html>`, `data-bs-theme`… Grep the CSS for `[data-theme`, `.dark`, `prefers-color-scheme`. A theme driven only by `prefers-color-scheme` cannot be toggled by the kit; say so in `SOURCE.md` and show light only.
3. **Write `intent/.mockups/<app>/snapshot.json`** (`<app>` is the application's name, e.g. `Storefront`):

   ```json
   {
     "app": "Storefront",
     "sourceRoot": "src/Storefront.Web",
     "prepare": "npm run build",
     "stylesheets": ["dist/assets/index.css", "node_modules/@fontsource/inter/index.css"],
     "exclude": ["public/img/photos"],
     "themeRule": "html.dark"
   }
   ```

   `prepare` is the app's own build, run in `sourceRoot` by the snapshot script and by the `<Mockup>` block's Build button. It must produce every listed stylesheet **from a fresh clone**, so include the package restore the stylesheets depend on (`npm ci && npm run build`, `dotnet build`, `bin/rails assets:precompile`, `python manage.py collectstatic --noinput`), and work in both `cmd.exe` and `sh`: chain with `&&`, no `./script`-only or shell-specific syntax. It must exit when done, never a watch or dev server. Leave it out when the stylesheets are committed.

   `stylesheets` are paths relative to `sourceRoot`, in the order the app loads them; their `@import`s and the fonts and images they name are followed from where each file is. `exclude` (files or folders relative to `sourceRoot`) leaves out what no screen needs, such as large photos or file-type icon sets: everything left in is embedded into `styles.css`, so its size is every mockup's weight. `themeRule` is one of `body[data-theme=dark]` (default), `html[data-theme=dark]`, `html.dark`, `body.<class>`, `html[<attr>=<value>]`. The kit draws pins green and highlights (`mk-new` outlines, the notes panel) electric blue; when either clashes with the app's own colours, set `"pins"` or `"highlight"` to one CSS colour (`"#e11d48"`). One mockup can override them in its `<style>` with `--mk-pin` and `--mk-highlight`.
4. **Run the snapshot script** from this skill's folder (`use_skill` reports it): `node <skill>/scripts/snapshot.mjs intent/.mockups/<app>/snapshot.json --prepare` (`--prepare-if-needed` runs `prepare` only when a stylesheet is missing). It writes `styles.css`, the one stylesheet a mockup links, with every import spliced in and every font (woff2 only) and image embedded; copies the kit to `kit/`; and writes `SOURCE.md` naming the branch and commit. It lists each file the stylesheets name that it could not find: add any a screen needs to the build, or `exclude` what is left out on purpose. Above 8 MB it warns; `exclude` images no mockup needs. It only rewrites those files; `templates/` is yours.
5. **Build the shell templates** in `intent/.mockups/<app>/templates/` from the app's real markup — the outer layout every screen sits in (navbar, sidebar, main area). Copy the markup from the app's source templates or the rendered DOM (`preview_evaluate` with `document.querySelector('#app').outerHTML` on the running app), strip framework attributes (`ng-*`, `v-*`, `x-*`, `data-reactid`), write repeated rows out by hand, and replace dynamic regions with `<div class="mk-placeholder">Designer canvas</div>`. An image the markup itself shows (a logo, an avatar) is copied into `templates/img/` and referenced from there; `styles.css` holds only what the CSS names. Start every template with:

   ```html
   <!DOCTYPE html>
   <html lang="en">
   <head>
       <meta charset="utf-8">
       <meta name="mockup-theme" content="html.dark">
       <link rel="stylesheet" href="../styles.css">
       <link rel="stylesheet" href="../kit/mockup.css">
       <script src="../kit/mockup.js" defer></script>
   </head>
   ```

6. **Check each template** in the preview in light and dark (see Part 2, step 5) against a screenshot of the real app. Fix the template, never the snapshot. Commit it as **What to commit** says.

Refresh later with the same command. If the app's markup changed, update the templates from the source rather than patching around the difference.

## Part 2: mock up a screen for a spec

In plan mode, `write_file` and `patch_file` are limited to `intent/.specs/<slug>/mockups/` (plus plan and spec documents), so Part 2 works but Part 1, which writes to `intent/.mockups/` and runs a shell, stays a developer task.

1. **Find the real screen.** Grep the app's source templates for a label or class you can see in the app, and read the component's markup and styles. Never invent class names: reuse the app's classes, so the mockup inherits the real look.
2. **Copy a template** from `intent/.mockups/<app>/templates/` to `intent/.specs/<slug>/mockups/<screen>/after.html` and fix the relative paths: from there the snapshot is `../../../../.mockups/<app>/` (so `../../../../.mockups/<app>/styles.css`, `…/kit/mockup.css`, `…/kit/mockup.js`). When the change alters an existing screen, make `before.html` (as it is today) too, from the same template.
3. **Build the screen** from the real markup. Only what the app genuinely lacks gets new CSS: a `<style>` block in the mockup, classes prefixed `<screen>-`, colours taken from the app's own variables. Mark anything new or changed with `mk-new` (a dashed outline reviewers can see).
4. **Pin each change to the criterion it realises:**

   ```html
   <span class="mk-pin" data-req="R3.2" title="R3.2 - Shows the unread count" style="top: 120px; left: 300px">1</span>
   ```

   `data-req` is the requirement or criterion id; in the `<Mockup>` block a click on the pin goes to that criterion. Inside anything with `overflow: hidden`, use `mk-pin mk-pin--inline` so it sits in the flow. Explain the pins in a side panel, so each screenshot carries its own notes:

   ```html
   <aside class="mk-notes-panel"><ol class="mk-notes">
       <li data-pin="1" data-req="R3.2"><b>Unread count</b>Shows how many items are new since the last visit.</li>
   </ol></aside>
   ```

   `?pins=off` hides pins, the notes panel and the `mk-new` outlines. The panel floats over the screen's right edge; to keep the screen clear of it, leave its room with `var(--mk-notes-space, 0px)` (e.g. `.app { margin-right: var(--mk-notes-space, 0px) }`), never a fixed width: the kit sets it to the panel's width, and to 0 with pins off, so a clean screen gets its full width back. In the `<Mockup>` block, hovering a pin shows its note.
5. **Look at it in light and dark.** The user sees the mockup in the `<Mockup>` block; you check it in a preview they never have to look at. `open_preview` with the mockup as a `file:///` URL (or a Windows path) and no `target`, so it stays a background tab, then `page.resize("desktop")` before checking — the tab is narrow and collapses app toolbars. Add `?theme=dark` for dark. With `run_preview_script`, check `console({ levels: ["error"] })` for missing files and take a `screenshot()` of each state (light, dark, and `?pins=off` for a clean copy) to check it. Keep them only when the user wants them, e.g. for a pull request read outside Intent: copy each PNG to `intent/.specs/<slug>/mockups/<screen>/screenshots/<before|after>-<light|dark>[-clean].png`. The `<Mockup>` block renders the HTML itself, so committed screenshots only duplicate it. A developer can serve the mockups instead with `node <skill>/scripts/serve.mjs` (http://localhost:5510/.specs/<slug>/mockups/<screen>/after.html).
6. **Show it in the chat and ask.** Whenever you build or change mockups, put the same block for each in one reply, its paths relative to the folder you work in, so the user sees the screens without opening anything. Then ask in that reply's text whether to approve each, change something or leave it out, and end the turn there: never `ask_user_question`, whose card covers the reply and hides the mockup.

   ```mdx
   <Mockup src="intent/.specs/<slug>/mockups/inbox/after.html" before="intent/.specs/<slug>/mockups/inbox/before.html" title="Inbox" />
   ```

   Write nothing else in that reply that MDX would misread (a bare `<` or `{` in prose). The user can comment on the mockup itself: a comment arrives as `Comments on the mockup …` with one line per comment naming the element clicked (`button.btn-primary "Save" in .toolbar`) and its position in page pixels. Find that element in the HTML, make the change, show the mockup again and ask again.
7. **Embed it** once the user has approved it, in the spec's `requirements.mdx` (load `authoring-rich-documents` and read its `<Mockup>` reference first), in its `## Screens` section above `## Requirements`, with `write_spec` (under `sdd-requirements`, as part of its own requirements write):

   ```mdx
   <Mockup id="inbox-mockup" src="mockups/inbox/after.html" before="mockups/inbox/before.html" title="Inbox" caption="Unread count and filters (R3)" />
   ```

   `src` and `before` are relative to the document.

## Rules

- **Never edit the snapshot by hand** (`styles.css`, `kit/`, `SOURCE.md`). Re-run the script.
- **A mockup is a picture, not a prototype.** A little script for a toggle or an open/closed state is fine; no data fetching, no framework, no network (the `<Mockup>` frame blocks every request anyway).
- **Every asset comes from the snapshot or sits beside the mockup.** Nothing outside the repository is embedded, and remote URLs (CDN fonts, images) are blocked in the block: copy them into the snapshot.
- **Show both themes** whenever the app has two. A mockup that only works in light is a bug.
- **Cite ids that exist.** A pin's `data-req` must name a requirement or criterion in the spec, or it goes nowhere.
