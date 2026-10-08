# `run_preview_script` — the `page` API

Full reference for the script body `run_preview_script` runs. The skill itself carries the six-line sketch you need for the common case; read this one when you need exact semantics, or when something did not behave as you expected.

## Shape of a script

The body is an **async function body**. `await` every `page` call, and `return` the value you want back — that return value is the result.

```js
await page.goto("/checkout");
await page.waitForSelector("form.checkout");
await page.type("ACME10", { into: "input[name=code]", clearFirst: true, submit: true });
await page.waitForText("Discount applied", { within: ".summary" });
return { total: await page.textOf(".order-total") };
```

There is **no `page.evaluate()`**. Raw in-page JavaScript is `preview_evaluate`, a separate tool with a higher approval level, and keeping the two apart is what makes the approval levels honest.

Available globals: `page`, `console`, `setTimeout` / `clearTimeout` / `setInterval` / `clearInterval`, and the JavaScript language. No `require`, no `process`, no filesystem.

## Targets

Anything that takes a `target` accepts either:

- a **CSS selector** — `"button[type=submit]"`, `"#code"`, `".row:nth-child(2) a"`. Resolved fresh, scrolled into view, then clicked at its centre.
- a **coordinate** — `{ x: 640, y: 120 }`, as reported by `preview_look`.

**Prefer selectors.** A coordinate is stale the moment the page re-renders; a selector is re-resolved on every call. Use a coordinate only for something that has no stable selector — a canvas, a chart, a third-party embed.

A selector that matches nothing, or matches something invisible, **throws**. Every member throws on failure rather than returning `false`, so a script that runs to completion genuinely did what it says.

## Reading

| Call | Returns |
| --- | --- |
| `snapshot()` | The same text outline `preview_look` returns. |
| `screenshot()` | `{ index, width, height }`; the PNG path is reported in the tool result. Max 5 per script. |
| `console({ clear = true, levels })` | Page console entries `{ level, text, url, line }`. `clear: true` returns only what is new since the last read in this script; `levels: ["error", "network"]` filters. |
| `state()` | `{ url, title, isLoading, canGoBack, canGoForward, viewportWidth, viewportHeight, viewportPreset }` |
| `url()` / `title()` | Shorthands for the corresponding `state()` fields. |
| `find(sel)` | One element: `{ selector, role, name, value, text, x, y, visible, disabled, checked }`. Throws if nothing matches. |
| `findAll(sel, limit = 50)` | The same shape, as an array. Empty array if nothing matches — this one does not throw. |
| `exists(sel)` | `boolean`. |
| `textOf(sel)` | Trimmed `innerText`. Throws if nothing matches. |
| `valueOf(sel)` | A form field's current value. Throws if nothing matches or it has no value. |
| `count(sel)` | How many elements match. |

Console levels are `log`, `info`, `warn`, `error`, `debug`, `pageerror` (an uncaught exception) and `network` (a failed request).

## Interacting

| Call | Notes |
| --- | --- |
| `click(target, { button = "left", clickCount = 1 })` | `button` is `left` / `right` / `middle`; `clickCount: 2` double-clicks. The synthetic click is preceded by a mouse move, so hover-revealed menus behave as they do for a real user. |
| `type(text, { into, clearFirst, submit })` | `into` is a target to click first (the usual case); omit it to type into whatever has focus. `clearFirst` selects-all and deletes so the field is replaced rather than appended to. `submit` presses Enter afterwards — forms listen for `keydown`, so this is a real key event. |
| `pressKey(combo)` | `Enter`, `Tab`, `Escape`, `Backspace`, `Delete`, `ArrowUp/Down/Left/Right`, `Home`, `End`, `PageUp`, `PageDown`, `Space`, or a single character. Prefix modifiers with `+`: `"Control+a"`, `"Shift+Tab"`. **F12 and the DevTools chords are rejected** — they would open DevTools, which takes the single debugger client this script is driving the page with. |
| `scroll(deltaY = 600, { deltaX, at })` | Positive `deltaY` scrolls down. `at` targets a specific scrollable region; the default is the viewport centre, because `0,0` is usually a fixed header that swallows the wheel event. |
| `scrollTo(sel)` | Scrolls an element into view without a wheel event. |

## Navigating and resizing

| Call | Notes |
| --- | --- |
| `goto(url)` | A full URL or a path like `"/checkout"`. Only loopback origins and the origin the preview was opened at are allowed. |
| `back()` / `forward()` / `reload()` | Throw when there is nothing to go back or forward to. |
| `resize(preset)` | `"desktop"` (1280×800), `"tablet"` (834×1112), `"mobile"` (390×844). |
| `resize(width, height)` | An explicit viewport. **`resize(0, 0)` clears the emulation** and returns the page to the real pane size. |

All five return the new `state()`. After a resize, every coordinate you were holding is wrong — re-`find` anything you still need.

## Waiting

This is what makes a script reliable rather than a race. Each takes a `timeout` in ms (default 10000) and is additionally bounded by the script's own budget.

| Call | Notes |
| --- | --- |
| `waitForSelector(sel, { state = "visible", timeout })` | `state` is `visible`, `hidden`, `attached` or `detached`. |
| `waitForText(text, { within, timeout })` | Substring match. `within` scopes it to a selector instead of the whole body. |
| `waitForLoad({ timeout })` | Until the page stops loading. |
| `waitForConsole(pattern, { levels, timeout })` | Returns the matching entry. `pattern` is a substring, or `"/regex/flags"` for a regex. Reads without draining, so it does not steal messages from `console()`. |
| `sleep(ms)` | Last resort. Prefer a `waitFor*` — a sleep is either too short (flaky) or too long (slow). |

`reveal()` raises a **window** preview without taking focus. It happens automatically at the start of a script and around navigation and screenshots; call it explicitly only to draw the user's attention. A **tab** preview is never switched to — it keeps working in the background, where a screenshot takes a second or two longer.

## Timeouts, errors and partial runs

`timeoutMs` bounds the whole script (default 30000, max 120000). Four layers enforce it — each CDP command, in-page execution, a cooperative check inside every `page` call, and finally terminating the worker — so an infinite loop cannot wedge Intent Architect.

**A timed-out script has already performed part of its work.** The result lists the actions it *attempted*, in order — each is narrated before it is dispatched, so the last entry may name a step whose effect landed even though the call never returned — and names what it was attempting when the budget ran out. Read that and continue from where it stopped. Re-running blindly submits the form a second time.

Errors thrown by **your own code**, and syntax errors, report against `preview_script.js` at the line and column **of the script you wrote**, so `preview_script.js:4:11` is line 4 of your body. A failure *inside* a `page.*` call — by far the most common one, a selector that matched nothing — carries **no line attribution**: it crosses back from the host as a message, so the stack is that message. Place it with the `actions` log instead; the last entry is the call that failed.

Only one script runs at a time **per preview**. A second on the same preview is rejected rather than queued: the preview has one debugger client, and its premises — coordinates, URL, page state — were computed against a page the first is actively changing. Scripts on two different previews run side by side.

## The four result channels

| Channel | What it is |
| --- | --- |
| `Result` | What the script returned. |
| `Actions` | An ordered narration of what actually happened (`click "#submit" @640,120`, `goto /checkout`). |
| `Script output` | Your own `console.log`. |
| `Page console` | The **application's** console, page errors and failed requests, for the whole run — including across a navigation, which the ordinary console buffer drops. |

## Worked examples

**Fill and submit a form, then assert the outcome:**

```js
await page.goto("/orders/new");
await page.waitForSelector("form#order");
await page.type("ACME Corp", { into: "#customer" });
await page.type("3", { into: "#quantity", clearFirst: true });
await page.click("button[type=submit]");
await page.waitForText("Order created");
return { url: await page.url(), reference: await page.textOf(".order-reference") };
```

**Check a list rendered the rows the API returned:**

```js
await page.goto("/products");
await page.waitForSelector("table tbody tr");
const rows = await page.findAll("table tbody tr td:first-child");
return { count: rows.length, names: rows.map(r => r.text) };
```

**Check responsiveness at three widths:**

```js
const results = [];
for (const preset of ["desktop", "tablet", "mobile"]) {
    await page.resize(preset);
    await page.waitForSelector("nav");
    results.push({ preset, burgerVisible: (await page.find("nav .burger")).visible });
}
await page.resize(0, 0);
return results;
```

**Diagnose a failing save:**

```js
await page.click("#save");
const failure = await page.waitForConsole("/40[0-9]|50[0-9]/", { levels: ["network", "error"], timeout: 5000 });
return { failure, formError: await page.textOf(".validation-summary").catch(() => null) };
```

## Making a task previewable

`open_preview` can only discover a URL if `tasks.json` says what the task serves. Two fields:

- **`previewUrl`** — a fixed URL, for a server whose port is set by configuration.
- **`urlPattern`** — a regex whose **first capture group** is the URL, scanned over the task's output. Use this whenever the port is not fixed in advance (Vite walks up from 5173 when the port is taken; `dotnet run` picks from `launchSettings.json`).

```jsonc
"start-ui": {
  "label": "Start UI",
  "command": "npm run dev",
  "isBackground": true,
  "readyPattern": "ready in \\d+",
  "urlPattern": "Local:\\s+(https?://\\S+)"
},
"start-api": {
  "label": "Start API",
  "command": "dotnet run",
  "isBackground": true,
  "readyPattern": "Application started",
  "urlPattern": "Now listening on:\\s+(https?://\\S+)"
}
```

`isBackground: true` is what marks a task as a long-lived server rather than a one-shot command. If you find yourself passing an explicit URL to `open_preview` every time, offer to add these fields.

## Typings

```ts
type Target = string | { x: number, y: number };

interface ElementInfo {
    selector: string;
    role: string;          // "button", "input:text", or an explicit ARIA role
    name: string;          // aria-label / placeholder / title / label / text
    value?: string;
    text: string;
    x: number; y: number;  // viewport centre, the coordinate a click targets
    visible: boolean;
    disabled: boolean;
    checked?: boolean;
}

interface ConsoleEntry { level: string; text: string; url?: string; line?: number }

interface PageState {
    url: string; title: string; isLoading: boolean;
    canGoBack: boolean; canGoForward: boolean;
    viewportWidth: number; viewportHeight: number; viewportPreset: string;
}

declare const page: {
    snapshot(): Promise<string>;
    screenshot(): Promise<{ index: number, width: number, height: number }>;
    console(options?: { clear?: boolean, levels?: string[] }): Promise<ConsoleEntry[]>;
    state(): Promise<PageState>;
    url(): Promise<string>;
    title(): Promise<string>;

    find(selector: string): Promise<ElementInfo>;
    findAll(selector: string, limit?: number): Promise<ElementInfo[]>;
    exists(selector: string): Promise<boolean>;
    textOf(selector: string): Promise<string>;
    valueOf(selector: string): Promise<string>;
    count(selector: string): Promise<number>;

    click(target: Target, options?: { button?: "left" | "right" | "middle", clickCount?: number }): Promise<void>;
    type(text: string, options?: { into?: Target, clearFirst?: boolean, submit?: boolean }): Promise<void>;
    pressKey(combination: string): Promise<void>;
    scroll(deltaY?: number, options?: { deltaX?: number, at?: Target }): Promise<void>;
    scrollTo(selector: string): Promise<void>;

    goto(url: string): Promise<PageState>;
    back(): Promise<PageState>;
    forward(): Promise<PageState>;
    reload(): Promise<PageState>;
    resize(preset: string): Promise<PageState>;
    resize(width: number, height: number): Promise<PageState>;

    waitForSelector(selector: string, options?: { state?: "attached" | "detached" | "visible" | "hidden", timeout?: number }): Promise<void>;
    waitForText(text: string, options?: { within?: string, timeout?: number }): Promise<void>;
    waitForLoad(options?: { timeout?: number }): Promise<void>;
    waitForConsole(pattern: string, options?: { levels?: string[], timeout?: number }): Promise<ConsoleEntry>;
    sleep(ms: number): Promise<void>;

    reveal(): Promise<void>;
};
```
