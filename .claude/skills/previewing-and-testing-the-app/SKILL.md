---
description: Load when something needs to be seen working in the user's running application rather than asserted — verifying a UI or behavioural change end to end, testing or checking a screen, reproducing a bug the user reported, or when a dev server is already running. Covers the embedded browser preview of the user's own app, not Intent Architect itself.
short-description: Run the user's app and verify it in the embedded browser preview (open_preview, preview_look, run_preview_script).
intent-updates: automatic
requiredTools:
  - run_task
  - kill_command
  - read_file
  - open_preview
  - preview_look
  - preview_screenshot
  - run_preview_script
  - preview_evaluate
---

# Previewing and Testing the App

An **app preview** is an embedded browser showing the user's running application. You drive it with a script, read what happened, and fix what is broken — so you verify your own changes instead of asserting they work.

It opens as a **background tab** in Intent Architect by default — it never takes the user's focus or switches their tab; they click it when they want to watch. Pass `target: "window"` for a separate window beside Intent Architect instead, e.g. so the user can watch while working in a designer. The user can move a preview between the two from its toolbar; reopening without a `target` leaves it wherever they put it.

**This is the user's application, not Intent Architect.** Only loopback URLs (`localhost`, `127.x`) are previewed, plus whatever origin a preview was explicitly opened at.

## The loop

**Start the server → open the preview → look → script an interaction → look → fix → reload → re-test.**

### 1. Start the dev server

Run the application's dev-server task with `run_task` and **`stopWhenReady: false`** — the default (`true`) stops the watcher the moment it reports ready, which is right for "does it build?" and wrong for "leave the app running".

The result tells you three things about a task left running: the **URL** it bound, the **live log file**, and the **handle** to stop it with. Do not start a second server: check the workspace context's `runningServers` first — if one is already up, use it.

If no URL is reported, the task has no `previewUrl` / `urlPattern` in `tasks.json` — see [references/preview-api.md](references/preview-api.md). Pass the URL to `open_preview` explicitly this time and mention the gap to the user.

### 2. Open the preview

`open_preview` with no `url` uses the URL the running task reported. Pass a `url` to land on a specific screen (`http://localhost:5173/checkout`).

It returns a **preview id**. Opening an origin you already preview navigates that preview rather than adding another tab, so call it freely to jump to a screen.

### Several previews at once

Open a second origin (say the API's Swagger UI beside the SPA) and you have two previews. Every other preview tool takes an optional **`previewId`**; without one it acts on the preview you used most recently. **Pass the id whenever more than one is open** — the workspace context's `appPreviews` lists them. Previews are per conversation: another chat's preview of the same app is its own tab and never yours to drive.

### 3. Look before you touch

**`preview_look` first, always.** It returns a text outline of every visible interactive element and heading, each with its click coordinate, followed by the console:

```
h1 "Checkout" @640,120
input:text "Discount code" value="" @512,340
button "Apply discount" @760,340

── Console ──
[network] POST /api/discounts 400 (Bad Request)
```

Outline and console are one call because neither is trustworthy alone — **a page that looks correct is routinely failing in the console**, and a 400 from a DTO missing the property you just modelled shows up there and nowhere else. Each look drains the console, so the next one reports only what is new.

`preview_screenshot` writes a PNG and returns its path for you to read. It costs far more than a look and tells you less about what to click. **Take one only when the rendering itself is the question** — layout, styling, spacing, "does this look right".

The outline lists only what is **on screen**. Scroll and look again to reach the rest.

### 4. Interact — with `run_preview_script`

One script, one turn. The intermediate state never enters your context, which is the point: click → look → click → look is four turns and four DOM dumps for what a script does in one.

```js
await page.click("#apply-discount");
await page.type("ACME10", { into: "input[name=code]", clearFirst: true, submit: true });
await page.waitForSelector(".order-total");
return { total: await page.textOf(".order-total"), errors: await page.console({ levels: ["error"] }) };
```

The whole surface, in six lines:

```
snapshot() screenshot() console({clear,levels}) state() url() title()
find(sel) findAll(sel) exists(sel) textOf(sel) valueOf(sel) count(sel)
click(target,{button,clickCount}) type(text,{into,clearFirst,submit}) pressKey(combo)
scroll(deltaY,{deltaX,at}) scrollTo(sel) goto(url) back() forward() reload() resize(preset|w,h)
waitForSelector(sel,{state,timeout}) waitForText(t,{within}) waitForLoad() waitForConsole(pattern) sleep(ms)
reveal()
```

Everything is `await`ed, and a `target` is a **CSS selector** or `{x, y}`. **Prefer selectors** — a coordinate is stale the moment the page re-renders, a selector is not. Every call **throws** on failure rather than returning false, so a script that finishes did what it says.

`return` a value and it comes back as the result. Read [references/preview-api.md](references/preview-api.md) for full signatures, viewport presets, waiting semantics and worked examples.

### 5. Read what happened — every time

The script result already carries the page console for the whole run. `preview_look` afterwards to see the state it left the page in.

### 6. Fix and re-verify

Fix the model (or the code), re-run the Software Factory, apply the changes, then `await page.reload()` and repeat from step 3. A dev server with hot reload may have picked the change up already; reload anyway rather than reasoning about whether it did.

## Verify after you apply

When a dev server is running, `write_file` / `patch_file` / `apply_staged_file_changes` results carry a reminder that the app is live. **Act on it.** Applying generated code and reporting success without looking at the running app is the failure this whole capability exists to prevent.

Do not report a UI or behavioural change as done until you have seen it work in the preview.

## `run_preview_script` or `preview_evaluate`?

`run_preview_script` does what a **user at the keyboard** could do, and asks for approval once. `preview_evaluate` does what only the **page's own code** could do — read `document.cookie` or `localStorage`, reach an in-memory token store, POST to an endpoint the UI never offers — and asks every time.

So: script it if a person could click their way to it, which is nearly always. Reach for `preview_evaluate` only when you genuinely need the page's own privileges. There is deliberately no `page.evaluate()` in the script API; that separation is what makes the approval levels mean anything.

## When you are done

A dev server you started keeps running after your turn. **Stop it with `kill_command`** and the handle `run_task` gave you, unless the user is going to keep using it — say which you did.

## Gotchas

- **A timed-out script has already done part of its work.** The result says how far it got, action by action. Read that and continue from there — re-running blindly submits the form twice.
- **DevTools and the agent cannot both drive the page.** Chromium allows one debugger client, so calls fail with a clear message while the user has DevTools open on the preview. Ask them to close it.
- **Cookies and localStorage persist** across dev-server and app restarts, on purpose — you stay logged in. "Clear preview data" in the preview toolbar resets it if stale state is the problem.
- **Nothing is preview-only.** Everything you do here happens to the real running application against its real data.
